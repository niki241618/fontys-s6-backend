using Microsoft.EntityFrameworkCore;
using Shared;

namespace LoggingService;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.Services.AddDbContext<LoggingDbContext>(options =>
		{
			string connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
			options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
		}, ServiceLifetime.Singleton);

		builder.Services.AddSingleton<LoggingService>();
		builder.Services.AddSingleton<RabbitMqLogEventConsumer>();

		//Inject the RabbitMqLogger with the connection string
		builder.Services.AddScoped<RabbitMqLogger>(x => ActivatorUtilities.CreateInstance<RabbitMqLogger>(x, builder.Configuration.GetConnectionString("RabbitMqHostName")));

		builder.Services.AddHostedService<RabbitMqLogEventConsumer>();
		
		var app = builder.Build();
		
		app.UseSwagger();
        app.UseSwaggerUI();

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapControllers();
		
		app.Run();
	}
}