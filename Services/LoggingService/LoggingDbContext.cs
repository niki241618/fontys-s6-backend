using LoggingService.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoggingService;

public class LoggingDbContext: DbContext
{
	protected LoggingDbContext()
	{
	}

	public LoggingDbContext(DbContextOptions options) : base(options)
	{
	}
	
	public DbSet<LogEntity> Logs { get; set; }
}