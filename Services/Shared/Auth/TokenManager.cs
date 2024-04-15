// using System;
// using System.IdentityModel.Tokens.Jwt;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.Azure.Functions.Worker;
// using Newtonsoft.Json.Linq;
// using Salvo2_Functions.Business.Classes;
// using Salvo2_Functions.Exceptions;
// using Salvo2_Functions.Helpers;
//
// namespace Salvo2_Functions.Auth;
//
// /// <summary>
// /// Used to validate Auth0 Bearer token as well as Salvo API token.
// /// </summary>
// public class TokenManager
// {
//     private readonly JwtTokenValidator jwtTokenValidator;
//     private readonly ApiTokenValidator apiTokenValidator;
//
//     public TokenManager(JwtTokenValidator jwtTokenValidator, ApiTokenValidator apiTokenValidator)
//     {
//         this.jwtTokenValidator = jwtTokenValidator;
//         this.apiTokenValidator = apiTokenValidator;
//     }
//     
//     /// <summary>
//     /// Validates an API or Auth0 Bearer token from <see cref="FunctionContext"/>.
//     /// </summary>
//     /// <param name="context">The function context</param>
//     /// <exception cref="UnauthenticatedException">Thrown if token is invalid or not found</exception>
//     public async Task ValidateToken(FunctionContext context)
//     {
//         string token = GetToken(context);
//         
//         if (token.StartsWith("Bearer "))
//         {
//             try
//             {
//                 JwtSecurityToken jwt = await jwtTokenValidator.ValidateJwt(token); //Will throw if invalid
//                 var claims = jwt.Claims
//                     .Where(claim => claim.Type.Equals("permissions"))
//                     .Select(claim => claim.Value)
//                     .ToArray();
//                 
//                 var companyIdClaim = jwt.Claims.FirstOrDefault(claim => claim.Type.Equals("company_id"));
//                 int companyId = companyIdClaim != null ? int.Parse(companyIdClaim.Value) : -1;
//                     
//                 context.SetUserClaims(claims);
//                 context.SetCompanyId(companyId);
//             }
//             catch (Exception)
//             {
//                 throw new UnauthenticatedException("Invalid Bearer token");
//             }
//         }
//         else if (token.Length > 0)
//         {
//             ApiKey apiKey = await apiTokenValidator.ValidateToken(token); //Will throw if invalid
//             context.SetUserClaims(apiKey.Claims);
//             context.SetCompanyId(apiKey.CompanyId);
//             if (apiKey.Extras.TryGet("msisdn", out string msisdn))
//             {
//                 context.SetMsisdn(msisdn);
//             }
//         }
//         else
//         {
//             throw new UnauthenticatedException("No Bearer or API token provided");
//         }
//     }
//     
//     /// <summary>
//     /// Extracts the token from headers in function context
//     /// </summary>
//     /// <param name="context">Function context</param>
//     /// <returns></returns>
//     private string GetToken(FunctionContext context)
//     {
//         if (!context.BindingContext.BindingData.TryGetValue("Headers", out var headersObj))
//             return null;
//         if (headersObj is not string headersStr)
//             return null;
//
//         var headersJson = JObject.Parse(headersStr);
//         string token = headersJson["Authorization"]?.ToString();
//         return token ?? "";
//     }
//     
// }