using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared;

namespace LoggingService;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddControllers();
		builder.Services.AddAuthorization();
		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
			options.Audience = builder.Configuration["Auth0:Audience"];
		});
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(setup =>
		{
			// Include 'SecurityScheme' to use JWT Authentication
			var jwtSecurityScheme = new OpenApiSecurityScheme
			{
				BearerFormat = "JWT",
				Name = "JWT Authentication",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Scheme = JwtBearerDefaults.AuthenticationScheme,
				Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

				Reference = new OpenApiReference
				{
					Id = JwtBearerDefaults.AuthenticationScheme,
					Type = ReferenceType.SecurityScheme
				}
			};

			setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

			setup.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{ jwtSecurityScheme, Array.Empty<string>() }
			});
		});
		builder.Services.AddDbContext<LoggingDbContext>(options =>
		{
			string connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
			//options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
			options.UseSqlServer(connectionString);
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

		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
		
		app.Run();
	}
}