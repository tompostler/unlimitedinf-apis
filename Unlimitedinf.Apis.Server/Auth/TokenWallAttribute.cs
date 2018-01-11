using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Server.Models.Auth;

namespace Unlimitedinf.Apis.Server.Auth
{
    public class TokenWallAttribute : ActionFilterAttribute
    {
        private static MemoryCache tokenCache = new MemoryCache("TokenCache");
        // Default cache for 12ish hours
        private static CacheItemPolicy defaultCachePolicy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(12, 0, 0) };

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Providing a token via the URL will supersede checking for an Authorization header
            var urlToken = context.HttpContext.Request.Query.SingleOrDefault(kvp => kvp.Key.Equals("token", StringComparison.OrdinalIgnoreCase)).Value.SingleOrDefault();
            var noUrlToken = string.IsNullOrWhiteSpace(urlToken);

            // All the ways we can fail
            if (noUrlToken)
                if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
                    context.Result = new ContentResult() { StatusCode = (int)HttpStatusCode.Unauthorized, Content = "Authorization header with 'Token' scheme required." };
                else if (!context.HttpContext.Request.Headers["Authorization"].Single().StartsWith("Token "))
                    context.Result = new ContentResult() { StatusCode = (int)HttpStatusCode.Unauthorized, Content = "Authorization header 'Token' scheme required." };

            if (context.Result != null)
                return;
            string token = noUrlToken ? context.HttpContext.Request.Headers["Authorization"].Single().Substring("Token ".Length) : urlToken;
            Trace.TraceInformation("Token " + token);

            // Check the token, easy
            if (Contracts.Auth.Token.IsTokenExpired(token))
            {
                Trace.TraceInformation("Token is expired: based on token.");
                context.Result = new ContentResult() { StatusCode = (int)HttpStatusCode.Unauthorized, Content = "Token is expired." };
                return;
            }

            // Check if it's already in the cache
            MyPrincipal principal;
            if (tokenCache.Contains(token))
            {
                // Check the token, medium
                principal = tokenCache.Get(token) as MyPrincipal;
                if (principal == null)
                {
                    // We've seen this bad token before.
                    Trace.TraceInformation("Token does not exist: based on cache.");
                    context.Result = new ContentResult() { StatusCode = (int)HttpStatusCode.Unauthorized, Content = "Token does not exist." };
                    return;
                }
                else if (principal.Token.Expiration < DateTime.UtcNow)
                {
                    Trace.TraceInformation("Token is expired: based on cache.");
                    context.Result = new ContentResult() { StatusCode = (int)HttpStatusCode.Unauthorized, Content = "Token is expired." };
                    return;
                }
            }
            else
            {
                // Check the token, hard
                var result = await TableStorage.Instance.Auth.ExecuteAsync(TokenExtensions.GetExistingOperation(token));
                if (result.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    tokenCache.Add(token, false, defaultCachePolicy);
                    Trace.TraceWarning("Token does not exist: based on tablestorage.");
                    context.Result = new ContentResult() { StatusCode = (int)HttpStatusCode.Unauthorized, Content = "Token does not exist." };
                    return;
                }

                // Create principal
                var tokenEnt = (TokenEntity)result.Result;
                if (tokenEnt.Expiration < DateTime.UtcNow)
                {
                    Trace.TraceInformation("Token is expired: based on tablestorage.");
                    context.Result = new ContentResult() { StatusCode = (int)HttpStatusCode.Unauthorized, Content = "Token is expired." };
                    return;
                }
                principal = new MyPrincipal(new GenericIdentity(tokenEnt.Username), null) { Token = tokenEnt };
            }

            // Cache the principal based off of the token
            tokenCache.Add(token, principal, new CacheItemPolicy { AbsoluteExpiration = principal.Token.Expiration });

            // Stamp the current user
            Thread.CurrentPrincipal = principal;
            context.HttpContext.User = principal;

            await next();
        }
    }

    public class MyPrincipal : GenericPrincipal
    {
        public MyPrincipal(IIdentity identity, string[] roles)
            : base(identity, roles)
        {
        }

        public TokenEntity Token { get; set; }
    }
}