using System.Text.Json.Serialization;
using AudioService.Data;
using AudioService.Services;
using AudioService.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
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
	options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
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
app.UseErrorHandling();

app.MapControllers();

app.Run();
return;

void AddServices(WebApplicationBuilder builder)
{
	builder.Services.AddScoped<IBooksService, BooksService>();
}