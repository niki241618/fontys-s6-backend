using AudioService.Data;
using Microsoft.EntityFrameworkCore;

namespace AudioService.Extensions;

public static class ApplicationExtensions
{
	public static async Task ApplyMigrations(this WebApplication app)
	{
		Console.WriteLine("Applying pending migrations...");
		using var scope = app.Services.CreateScope();
		
		var services = scope.ServiceProvider;
		var dbContext = services.GetRequiredService<BooksContext>();
		await dbContext.Database.MigrateAsync();
	}
}