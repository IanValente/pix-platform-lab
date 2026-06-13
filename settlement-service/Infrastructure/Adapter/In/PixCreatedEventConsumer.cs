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
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            _logger.LogInformation($"[Settlement Service] JSON recebido: {message}");

            try
            {
                // 1. Desserializa o JSON para o nosso DTO
                // PropertyNameCaseInsensitive = true lida com a diferença de maiúsculas/minúsculas entre o Java e o C#
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var pixEvent = JsonSerializer.Deserialize<PixCreatedEventDto>(message, options);

                if (pixEvent != null)
                {
                    // 2. Cria um escopo isolado de Injeção de Dependência (Melhor prática para Background Services)
                    using var scope = _serviceProvider.CreateScope();
                    var processSettlementUseCase = scope.ServiceProvider.GetRequiredService<IProcessSettlementUseCase>();

                    // 3. Chama a Regra de Negócio (O Caso de Uso Hexagonal)
                    processSettlementUseCase.Execute(pixEvent.id, pixEvent.amount);
                    
                    _logger.LogInformation($"[Settlement Service] Liquidação processada com sucesso para o Pix: {pixEvent.id}");
                }

                // 4. Confirma pro RabbitMQ apagar a mensagem
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao processar mensagem: {ex.Message}");
                // Se der erro, joga de volta pra fila (Nack) para não perder o dinheiro!
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
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