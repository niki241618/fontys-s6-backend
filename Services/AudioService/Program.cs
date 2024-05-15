using System.Text.Json.Serialization;
using AudioService;
using AudioService.Data;
using AudioService.Extensions;
using AudioService.Services;
using AudioService.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.SharedMiddleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BooksContext>(options =>
{
	string connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
	try
	{
		options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
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
builder.Services.AddCors(p => p.AddPolicy("Corsapp", builder =>
{
	builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));
//Configure Kestrel server
builder.WebHost.ConfigureKestrel(options =>
{
	options.Limits.MaxRequestBodySize = 500 * 1024 * 1024; // 500 MB
});

AddServices(builder);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("Corsapp");
app.UseErrorHandling();
app.UseHttpsRedirection();
app.UseAuthorization();
// app.UseLogging(new ConsoleLogger());
app.UseErrorHandling();

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