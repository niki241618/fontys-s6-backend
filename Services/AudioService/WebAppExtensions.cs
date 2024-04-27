using AudioService.Data;

namespace AudioService;

public static class WebAppExtensions
{
	public static WebApplication UseSeeding(this WebApplication app)
	{
		if (app.Configuration["SeedDatabase"] != "true")
			return app;
		
		using var scope = app.Services.CreateScope();
	
		var dbContext = scope.ServiceProvider.GetRequiredService<BooksContext>();
		new DbSeeder(dbContext).Seed(); // Your custom seed function

		return app;
	}
}