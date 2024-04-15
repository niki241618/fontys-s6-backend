using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Classes;


namespace Shared;

public class RabbitMqLogger: ILogging
{
	private readonly ConnectionFactory factory;

	public RabbitMqLogger(string hostname)
	{
		factory = new ConnectionFactory() { HostName = hostname };
	}

	public void Log(Log log)
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
		
		var logJson = JsonSerializer.Serialize(log);
		var encodedLog = Encoding.UTF8.GetBytes(logJson);
		channel.BasicPublish("", "logs", null, encodedLog);
	}
}