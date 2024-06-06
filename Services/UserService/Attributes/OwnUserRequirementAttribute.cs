using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace User_Service.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class OwnUserRequirementAttribute : Attribute, IAuthorizationFilter
{
    private readonly string ignoredClaim;
    private readonly string userIdParamName;

    public OwnUserRequirementAttribute(string ignoredClaim = "role:admin", string userIdParamName = "userId")
    {
	    this.ignoredClaim = ignoredClaim;
	    this.userIdParamName = userIdParamName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
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
    	if (!routeData.ContainsKey(userIdParamName))
    	{
    		context.Result = new BadRequestResult();
    		return;
    	}
	    
	    string requestedUserId = routeData[userIdParamName].ToString();

    	if (string.IsNullOrWhiteSpace(requestedUserId))
    	{
    		context.Result = new BadRequestResult();
    		return;
    	}

	    if (!userId.Equals(requestedUserId))
	    {
		    context.Result = new ForbidResult();
		    return;
	    }
    }
}
