using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shared.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresClaimAttribute: Attribute, IAuthorizationFilter
{
	private const string CLAIM_NAME = "permissions";
	private readonly string requiredClaim;

	public RequiresClaimAttribute(string requiredClaim)
	{
		this.requiredClaim = requiredClaim;
	}

	public void OnAuthorization(AuthorizationFilterContext context)
	{
		if(!context.HttpContext.User.HasClaim(x => x.Type == CLAIM_NAME && x.Value == requiredClaim))
			context.Result = new ForbidResult();
	}
}