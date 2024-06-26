using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Auth;
using Shared.Classes;

namespace LoggingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
	private readonly LoggingService loggingService;
	private readonly RabbitMqLogger logger;

	public LogsController(LoggingService loggingService, RabbitMqLogger logger)
	{
		this.loggingService = loggingService;
		this.logger = logger;
	}

	[HttpGet]
	[Authorize]
	[RequiresClaim("role:admin")]
	public async Task<IActionResult> GetLogs()
	{
		List<Log> logs = await loggingService.GetLogs();
		
		return Ok(logs);
	}
	
	[HttpPost]
	[Authorize]
	[RequiresClaim("role:admin")]
	public IActionResult Test([FromBody] string message = "Test")
	{
		Log log = new Log(message);
		logger.Log(log);
		
		return Ok(log);
	}
}