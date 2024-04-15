// using System;
// using System.Collections.Generic;
// using System.Net.Http;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Caching.Memory;
// using Microsoft.IdentityModel.Protocols;
// using Microsoft.IdentityModel.Protocols.OpenIdConnect;
// using Microsoft.IdentityModel.Tokens;
//
// namespace Salvo2_Functions.Auth;
//
// /// <summary>
// /// Provides JSON Web Key Sets used by Auth0 to validate signature of Bearer token 
// /// </summary>
// public class JwtKeyProvider
// {
//     private readonly string jwksUrl;
//     private readonly IMemoryCache cache;
//
//     public JwtKeyProvider(string jwtdomain, IMemoryCache cache)
//     {
//         this.jwksUrl = $"https://{jwtdomain}/.well-known/openid-configuration";
//         this.cache = cache;
//     }
//
//     /// <summary>
//     /// Returns cached JWKS or fetch them if not found. Keys are cached for 6 hours
//     /// </summary>
//     /// <remarks>NOTE: Currently there is no way to check if cached keys are still valid. In case of key rotation, valid tokens may be considered as invalid, because old (cached) signing keys will be used for validation</remarks>
//     /// <returns>JWK </returns>
//     public async Task<ICollection<SecurityKey>> GetJwksAsync()
//     {
//         if (cache.TryGetValue("SecurityKeysCacheKey", out ICollection<SecurityKey> cachedSecurityKeys))
//         {
//             return cachedSecurityKeys;
//         }
//         //Keys are not found in cache or have expired
//         ICollection<SecurityKey> keys = await FetchKeys();
//         CacheSecurityKeys(keys);
//         return keys;
//     }
//
//     private async Task<ICollection<SecurityKey>> FetchKeys()
//     {
//         var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>
//         (
//             jwksUrl, 
//         new OpenIdConnectConfigurationRetriever(),
//         new HttpDocumentRetriever()
//         );
//
//         var discoveryDocument = await configurationManager.GetConfigurationAsync();
//         var signingKeys = discoveryDocument.SigningKeys;
//         return signingKeys;
//     }
//     
//     private void CacheSecurityKeys(ICollection<SecurityKey> securityKeys)
//     {
//         var cacheEntryOptions = new MemoryCacheEntryOptions
//         {
//             AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
//         };
//         cache.Set("SecurityKeysCacheKey", securityKeys, cacheEntryOptions);
//     }
// }