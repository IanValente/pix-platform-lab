using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SettlementService.Application.Port.In;
using SettlementService.Infrastructure.Adapter.In.Dto;

namespace SettlementService.Infrastructure.Adapter.In;

public class PixCreatedEventConsumer : BackgroundService
{
    private readonly ILogger<PixCreatedEventConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private IModel _channel;
    private const string QueueName = "pix.created.queue";

    // Injetamos o IServiceProvider para podermos instanciar o Caso de Uso de forma segura no Background
    public PixCreatedEventConsumer(ILogger<PixCreatedEventConsumer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        InitRabbitMq();
    }

    private void InitRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            // Tenta pegar o nome da rede do Docker. Se for nulo (rodando fora do Docker), usa localhost.
            HostName = Environment.GetEnvironmentVariable("RabbitMq__HostName") ?? "localhost"
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // 1. Declaramos a fila do cemitério (DLQ)
        var dlqName = "pix.created.dlq";
        _channel.QueueDeclare(queue: dlqName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        // 2. Criamos as regras (Arguments) da fila principal
        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" }, // Usa a exchange default (direta)
            { "x-dead-letter-routing-key", dlqName } // Se rejeitar, manda pra DLQ
        };

        // 3. Declaramos a fila principal injetando as regras
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: args);
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += (model, ea) =>
        {
            // 1. Extrai o Correlation ID dos cabeçalhos (Headers) do RabbitMQ
            string correlationId = Guid.NewGuid().ToString(); // Valor padrão caso não venha
            if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.ContainsKey("X-Correlation-ID"))
            {
                var bytes = (byte[])ea.BasicProperties.Headers["X-Correlation-ID"];
                correlationId = Encoding.UTF8.GetString(bytes);
            }

            // 2. A MÁGICA: Cria um escopo de log. Todo logger executado dentro deste 'using' terá o ID embutido!
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                _logger.LogInformation($"[Settlement Service] Iniciando processamento da mensagem.");

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var pixEvent = JsonSerializer.Deserialize<PixCreatedEventDto>(message, options);

                    if (pixEvent != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var processSettlementUseCase = scope.ServiceProvider.GetRequiredService<IProcessSettlementUseCase>();
                        processSettlementUseCase.Execute(pixEvent.id, pixEvent.amount);
                    }

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); 
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro crítico no processamento. Movendo para DLQ.");
                    
                    // O requeue: false ativará a regra x-dead-letter que configuramos acima!
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}