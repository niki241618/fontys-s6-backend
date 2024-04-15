// using System;
// using System.Collections.Generic;
// using System.IdentityModel.Tokens.Jwt;
// using System.Linq;
// using System.Net;
// using System.Net.Http;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Caching.Memory;
// using Microsoft.IdentityModel.Tokens;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
//
// namespace Salvo2_Functions.Auth;
//
// /// <summary>
// /// Used to make request to Auth0 Management API.
// /// </summary>
// public class ManagementApi
// {
//     private readonly IMemoryCache memoryCache;
//     private const string TOKEN_KEY = "TOKEN";
//
//     public ManagementApi(IMemoryCache memoryCache)
//     {
//         this.memoryCache = memoryCache;
//     }
//
//     /// <summary>
//     /// Sends request to the Auth0 Management API
//     /// </summary>
//     /// <param name="method">HTTP method (GET,POST,UPDATE,etc.)</param>
//     /// <param name="endpoint">Endpoint of Management API</param>
//     /// <param name="body">Json Body. Optional</param>
//     /// <param name="queryParams">Query params for the call. Optional</param>
//     /// <returns>Result of the call</returns>
//     public async Task<HttpResponseMessage> SendRequest(HttpMethod method, string endpoint, object body = null, Dictionary<string, string> queryParams = null)
//     {
//         var url = $"https://{Environment.GetEnvironmentVariable("JWT_DOMAIN")}/api/{endpoint}";
//         var client = new HttpClient();
//         var request = new HttpRequestMessage(method, url);
//         
//         request.Headers.Add("Accept", "application/json");
//         request.Headers.Add("Authorization", $"Bearer {await GetToken()}");
//         
//         AppendQueryParams(ref url, queryParams);
//         
//         if (body != null)
//         {
//             request.Content = new StringContent(JsonConvert.SerializeObject(body), null, "application/json");
//         }
//         
//         var response = await client.SendAsync(request);
//         return response;
//     }
//     
//     /// <summary>
//     /// Checks if there is cached valid Auth0 Management API token and returns it. Otherwise will fetch new token from Auth0
//     /// </summary>
//     /// <returns>Valid Auth0 Management API token </returns>
//     private async Task<string> GetToken()
//     {
//         if (memoryCache.TryGetValue(TOKEN_KEY, out string token) && TokenIsValid(token))
//         {
//             return token;
//         }
//         
//         string accessToken = await FetchToken();
//         CacheToken(accessToken);
//         return accessToken;
//     }
//
//     private async Task<string> FetchToken()
//     {
//         string domain = Environment.GetEnvironmentVariable("JWT_DOMAIN");
//         string clientId = Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID");
//         string clientSecret = Environment.GetEnvironmentVariable("AUTH0_CLIENT_SECRET");
//             
//         var client = new HttpClient();
//         var request = new HttpRequestMessage(HttpMethod.Post, $"https://{domain}/oauth/token")
//         {
//             Content = new StringContent($"{{\"grant_type\": \"client_credentials\",\n  \"client_id\": \"{clientId}\",\n  \"client_secret\": \"{clientSecret}\",\n  \"audience\": \"https://{domain}/api/v2/\"}}", null, "application/json")
//         };
//         var response = await client.SendAsync(request);
//         response.EnsureSuccessStatusCode();
//             
//         string responseBody = await response.Content.ReadAsStringAsync();
//         dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
//         return jsonResponse.access_token;
//     }
//
//     private bool TokenIsValid(string token)
//     {
//         JwtSecurityToken jwtSecurityToken;
//         try
//         {
//             jwtSecurityToken = new JwtSecurityToken(token);
//         }
//         catch (Exception)
//         {
//             return false;
//         }
//     
//         return jwtSecurityToken.ValidTo > DateTime.UtcNow;
//     }
//
//     private void CacheToken(string token)
//     {
//         var cacheEntryOptions = new MemoryCacheEntryOptions
//         {
//             //Token is usually valid for 24 hours, however it can be improved by specifying the exact
//             //validity time that can be acquired from the FetchToken()'s response
//             AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)  
//         };
//         
//         memoryCache.Set(TOKEN_KEY, token, cacheEntryOptions);
//     }
//
//     private void AppendQueryParams(ref string url, Dictionary<string, string> queryParams)
//     {
//         if(queryParams.IsNullOrEmpty())
//             return;
//
//         url = $"{url}?{string.Join("&", queryParams.Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"))}";
//     }
// }