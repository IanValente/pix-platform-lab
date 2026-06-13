using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SettlementService.Infrastructure.Adapter.In;

public class PixCreatedEventConsumer : BackgroundService
{
    private readonly ILogger<PixCreatedEventConsumer> _logger;
    private IConnection _connection;
    private IModel _channel;
    private const string QueueName = "pix.created.queue";

    public PixCreatedEventConsumer(ILogger<PixCreatedEventConsumer> logger)
    {
        _logger = logger;
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
            
            _logger.LogInformation($"[Settlement Service] Mensagem recebida do RabbitMQ: {message}");
            
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); 
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