using System.Text;
using RabbitMQ.Client;

namespace User_Service;

public class UserDeletionProducerRmq
{
	private readonly ConnectionFactory factory;

	public UserDeletionProducerRmq(IConfiguration configuration)
	{
		this.factory = new ConnectionFactory
		{
			HostName = configuration["ConnectionStrings:RabbitMqHostName"]
		};
	}
	
	public void MessageUserDeleted(string userId)
	{
		try
		{
			using var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();

			channel.QueueDeclare(
				queue: "user-deletion",
				durable: true,
				exclusive: false,
				autoDelete: false,
				arguments: null
			);
		
			var encodedUserId = Encoding.UTF8.GetBytes(userId);
			channel.BasicPublish("", "user-deletion", null, encodedUserId);
		}
		catch (Exception e)
		{
			Console.WriteLine("An error occurred while trying to send user-deletion message." + userId + "Error: " + e.Message);
		}
	}
}