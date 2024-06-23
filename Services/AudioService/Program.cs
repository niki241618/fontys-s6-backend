using System.Text.Json.Serialization;
using AudioService;
using AudioService.Consumers;
using AudioService.Data;
using AudioService.Extensions;
using AudioService.Services;
using AudioService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared;
using Shared.SharedMiddleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "CorsPolicy",
		x =>
		{
			x.AllowAnyOrigin()
				.AllowAnyHeader()
				.AllowAnyMethod();
		});
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddAuthorization();
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
builder.Services.AddDbContext<BooksContext>(options =>
{
	string connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
	try
	{
		options.UseSqlServer(connectionString);
		//options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
	}
	catch (Exception)
	{
		Console.WriteLine("An error occurred while connecting to the database.");
		try
		{
			var connString = Utils.ExtractDetailsFromConnectionString(connectionString);
			connString.TryGetValue("Server", out var server);
			connString.TryGetValue("Database", out var database);
			Console.WriteLine($"Host: {server ?? "Unknown"}");
			Console.WriteLine($"Database: {database ?? "Unknown"}");
		}
		catch (Exception)
		{
			Console.WriteLine($"An error occurred while extracting connection string details for connection string: {connectionString}.");
		}
		
		throw;
	}
});
builder.Services.AddRouting(options =>
{
	options.LowercaseUrls = true;
});
builder.Services.AddMvc().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
//Configure Kestrel server
builder.WebHost.ConfigureKestrel(options =>
{
	options.Limits.MaxRequestBodySize = 700 * 1024 * 1024; // 700 MB
});

builder.Services.AddHostedService<UserDeletionRmqConsumer>();

AddServices(builder);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


//app.UseHttpsRedirection(); //Will block HTTP connections!
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseErrorHandling();
app.UseLogging(new RabbitMqLogger(app.Configuration.GetConnectionString("RabbitMqConnection")));

app.MapControllers();

app.ApplyMigrations().GetAwaiter().GetResult();
app.UseSeeding();

app.Run();
return;

void AddServices(WebApplicationBuilder builder)
{
	builder.Services.AddScoped<IBooksService, BooksService>();
	builder.Services.AddScoped<IBooksFilesService, AzureBlobService>();
}