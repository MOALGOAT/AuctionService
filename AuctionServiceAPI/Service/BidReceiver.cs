using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuctionServiceAPI.Models;
using MongoDB.Bson.IO;

public class BidReceiver : BackgroundService
{
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BidReceiver> _logger;

    public BidReceiver(IServiceScopeFactory serviceScopeFactory, ILogger<BidReceiver> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        var factory = new ConnectionFactory { HostName = Environment.GetEnvironmentVariable("QueueHostName")}; // Use the hostname defined in Docker Compose
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "bid_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($" [x] Received {message}");
            await HandleMessageAsync(message);
        };
        _channel.BasicConsume(queue: "bid_queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(string message)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();
        await auctionService.ProcessMessageAsync(message);
    }

    public override void Dispose()
    {
        _channel.Close();
        _channel.Dispose();
        base.Dispose();
    }
}
