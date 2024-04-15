// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IdentityModel.Tokens.Jwt;
// using System.Threading.Tasks;
// using Microsoft.IdentityModel.Tokens;
// using Salvo2_Functions.Exceptions;
//
// namespace Salvo2_Functions.Auth;
//
// public class JwtTokenValidator
// {
//     private readonly JwtKeyProvider keyProvider;
//
//     public JwtTokenValidator(JwtKeyProvider keyProvider)
//     {
//         this.keyProvider = keyProvider;
//     }
//     
//     /// <summary>
//     /// Validates Bearer token
//     /// </summary>
//     /// <param name="token"></param>
//     /// <returns></returns>
//     public async Task<JwtSecurityToken> ValidateJwt(string token)
//     {
//         token = token.Substring("Bearer ".Length);
//
//         string domain = Environment.GetEnvironmentVariable("JWT_DOMAIN");
//         string issuer = $"https://{domain}/";
//         string audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
//         var securityKeys = await keyProvider.GetJwksAsync();
//         
//         var validationParameters = new TokenValidationParameters {
//             ValidateIssuer = true,
//             ValidIssuer = issuer,
//             ValidateAudience = true,
//             ValidAudience = audience,
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKeys = securityKeys,
//             ValidateLifetime = true,
//             ClockSkew = TimeSpan.Zero
//         };
//         
//         var tokenHandler = new JwtSecurityTokenHandler();
//         tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
//         
//         return (JwtSecurityToken)validatedToken;
//     }
//     
// }