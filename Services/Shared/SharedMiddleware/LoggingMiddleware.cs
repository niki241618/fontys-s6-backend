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
	private readonly ILogging logging;
	
	public LoggingMiddleware(RequestDelegate next, ILogging logging)
	{
		this.next = next;
		this.logging = logging;
	}

	public async Task Invoke(HttpContext ctx)
	{
		try
		{
			await next.Invoke(ctx);
		}
		catch (Exception e)
		{
			logging.Log(new Log(e.Message, Log.Type.ERROR));
			throw;
		}
	}
}

public static class LoggingMiddlewareExtensions
{
	public static IApplicationBuilder UseLogging(this IApplicationBuilder builder, ILogging logging)
	{
		return builder.UseMiddleware<LoggingMiddleware>(logging);
	}
}