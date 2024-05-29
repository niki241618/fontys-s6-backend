using System.Text;
using System.Text.Json;
using AudioService.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Shared.Classes;

namespace AudioService.Consumers;

public class UserDeletionRmqConsumer: BackgroundService
{
	private readonly ConnectionFactory factory;
	private readonly IBooksService booksService;
	
	public UserDeletionRmqConsumer(IConfiguration configuration, IBooksService booksService)
	{
		this.booksService = booksService;
		this.factory = new ConnectionFactory
		{
			HostName = configuration["ConnectionStrings:RabbitMqHostName"]
		};
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
					queue: "user-deletion",
					durable: true, //Ensure that even if the broker dies, the queue will not be lost.
					exclusive: false, 
					autoDelete: false,
					arguments: null
				);
		
				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += OnReceive;
				channel.BasicConsume("user-deletion", autoAck: true, consumer: consumer);
		
				Console.WriteLine("Listening for user deletions...");
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
				Console.WriteLine("Connection to RabbitMQ closed. Reconnecting...");
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
			string userId = JsonSerializer.Deserialize<string>(messageBody);
			await booksService.DeleteBooksForUser(userId);
		}
		catch (JsonException e)
		{
			Console.WriteLine("An exception was thrown while deserializing log's message body: " + e.Message);
			return;
		}
	}
}