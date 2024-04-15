using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shared.Exceptions;

namespace Shared.SharedMiddleware;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate next;
	public ExceptionHandlingMiddleware(RequestDelegate next)
	{
		this.next = next;
	}

	public async Task Invoke(HttpContext ctx)
	{
		try
		{
			await next.Invoke(ctx);
		}
		catch (NotFoundException e)
		{
			await ctx.AppendError(e.Message, HttpStatusCode.NotFound);
		}
		catch (AudioOasisException e)
		{
			await ctx.AppendError(e.Message);
		}
	}
	
}

public static class ExceptionHandlingMiddlewareExtensions
{
	public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<ExceptionHandlingMiddleware>();
	}

	public static async Task AppendError(this HttpContext ctx, string text, HttpStatusCode code = HttpStatusCode.BadRequest)
	{
		// Set response status code to 400 Bad Request
		ctx.Response.StatusCode = (int)code;

		// Set response body to the error message
		var errorMessage = JsonSerializer.Serialize(new { error = text });
		ctx.Response.ContentType = "application/json";
		await ctx.Response.WriteAsync(errorMessage);
	}
}