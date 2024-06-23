using LoggingService.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Classes;

namespace LoggingService;

public class LoggingService
{
	private readonly LoggingDbContext dbContext;

	public LoggingService(LoggingDbContext dbContext)
	{
		this.dbContext = dbContext;
		dbContext.Database.Migrate();
	}

	public async Task SaveLog(LogEntity logEntity)
	{
		dbContext.Logs.Add(logEntity);
		await dbContext.SaveChangesAsync();
	}

	public async Task<List<Log>> GetLogs()
	{
		return (await dbContext.Logs.ToListAsync())
			.Select(x => x.Convert())
			.ToList();
	}
}