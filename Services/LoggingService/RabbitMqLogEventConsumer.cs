using System.Text;
using System.Text.Json;
using LoggingService.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Shared.Classes;

namespace LoggingService;

public class RabbitMqLogEventConsumer: BackgroundService
{
	private readonly ConnectionFactory factory;
	private readonly LoggingService loggingService;

	public RabbitMqLogEventConsumer(LoggingService loggingService, IConfiguration configuration)
	{
		this.factory = new ConnectionFactory
		{
			HostName = configuration["ConnectionStrings:RabbitMqHostName"]
		};
		
		this.loggingService = loggingService;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		Console.WriteLine("Connecting to RabbitMQ...");
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var connection = factory.CreateConnection();
				using var channel = connection.CreateModel();
		
				channel.QueueDeclare(
					queue: "logs",
					durable: false,
					exclusive: false,
					autoDelete: false,
					arguments: null
				);
		
				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += OnReceive;
				channel.BasicConsume("logs", autoAck: true, consumer: consumer);
		
				Console.WriteLine("Listening for logs...");
				await Task.Delay(Timeout.Infinite, stoppingToken);
			}
			catch (BrokerUnreachableException e)
			{
				Console.WriteLine("Failed to connect to RabbitMQ. Retrying...");
				await Task.Delay(5000, stoppingToken); // Retry after 5 seconds
			}
			catch (OperationInterruptedException)
			{
				// Occurs when the connection is closed externally
				Console.WriteLine("Connection to RabbitMQ closed. Retrying...");
				await Task.Delay(5000, stoppingToken); // Retry after 5 seconds
			}
		}
	}
	private async void OnReceive(object? model, BasicDeliverEventArgs args)
	{
		Console.WriteLine("Received log.");
		var bodyBytes = args.Body.ToArray();
		var messageBody = Encoding.UTF8.GetString(bodyBytes);

		if (messageBody == null) 
			return;
			
		try
		{
			Log log = JsonSerializer.Deserialize<Log>(messageBody);
			if (log != null)
			{
				await loggingService.SaveLog(new LogEntity(log));
			}
			else
			{
				Console.WriteLine("Failed to deserialize log's message body.");
			}
		}
		catch (JsonException e)
		{
			Console.WriteLine("An exception was thrown while deserializing log's message body: " + e.Message);
			return;
		}
	}
}