using System.Security.Claims;
using AudioService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AudioService.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class OwnsBookRequirementAttribute : Attribute, IAsyncAuthorizationFilter
{
	private readonly string ignoredClaim;
	private readonly string bookIdParamName;

	public OwnsBookRequirementAttribute(string ignoredClaim="role:admin", string bookIdParamName = "id")
	{
		this.ignoredClaim = ignoredClaim;
		this.bookIdParamName = bookIdParamName;
	}

	public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
	{
		var user = context.HttpContext.User;
		if (!user.Identity.IsAuthenticated)
		{
			context.Result = new ForbidResult();
			return;
		}
		if(user.HasClaim("permissions", ignoredClaim))
			return;

		var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
		if (userIdClaim == null)
		{
			context.Result = new ForbidResult();
			return;
		}

		var userId = userIdClaim.Value;
		var routeData = context.RouteData.Values;
		if (!routeData.ContainsKey("id"))
		{
			context.Result = new BadRequestResult();
			return;
		}

		if (!int.TryParse(routeData[bookIdParamName].ToString(), out var bookId))
		{
			context.Result = new BadRequestResult();
			return;
		}

		var booksService = context.HttpContext.RequestServices.GetRequiredService<IBooksService>();
		var book = await booksService.GetBook(bookId);
		
		if(book.OwnerId != userId)
			context.Result = new ForbidResult();
	}
}