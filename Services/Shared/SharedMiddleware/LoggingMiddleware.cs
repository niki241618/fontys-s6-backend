using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shared.Classes;
using Shared.Exceptions;

namespace Shared.SharedMiddleware;

public class LoggingMiddleware
{
	private readonly RequestDelegate next;
	private readonly ILogger logger;
	
	public LoggingMiddleware(RequestDelegate next, ILogger logger)
	{
		this.next = next;
		this.logger = logger;
	}

	public async Task Invoke(HttpContext ctx)
	{
		try
		{
			await next.Invoke(ctx);
		}
		catch (Exception e)
		{
			logger.Log(new Log(e.Message, Log.Type.ERROR));
			throw;
		}
	}
}

public static class LoggingMiddlewareExtensions
{
	public static IApplicationBuilder UseLogging(this IApplicationBuilder builder, ILogger logger)
	{
		return builder.UseMiddleware<LoggingMiddleware>(logger);
	}
}