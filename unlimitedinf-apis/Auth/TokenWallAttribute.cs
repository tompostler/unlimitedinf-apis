using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Unlimitedinf.Apis.Models.Auth;

namespace Unlimitedinf.Apis.Auth
{
    public class TokenWallAttribute : AuthorizationFilterAttribute
    {
        private static MemoryCache tokenCache = new MemoryCache("TokenCache");
        // Default cache for 12ish hours
        private static CacheItemPolicy defaultCachePolicy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(12, 0, 0) };

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Providing a token via the URL will supersede checking for an Authorization header
            var urlToken = actionContext.Request.GetQueryNameValuePairs().SingleOrDefault(kvp => kvp.Key.Equals("token", StringComparison.OrdinalIgnoreCase)).Value;
            var noUrlToken = string.IsNullOrWhiteSpace(urlToken);

            // All the ways we can fail
            if (noUrlToken)
                if (actionContext.Request.Headers.Authorization == null)
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Authorization header with 'Token' scheme required." };
                else if (actionContext.Request.Headers.Authorization.Scheme != "Token")
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Authorization header 'Token' scheme required." };
                else if (string.IsNullOrWhiteSpace(actionContext.Request.Headers.Authorization.Parameter))
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Authorization header token parameter required." };

            if (actionContext.Response != null)
                return;
            string token = noUrlToken ? actionContext.Request.Headers.Authorization.Parameter : urlToken;
            Trace.TraceInformation("Token " + token);

            // Check the token, easy
            if (Contracts.Auth.Token.IsTokenExpired(token))
            {
                Trace.TraceInformation("Token is expired: based on token.");
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token is expired." };
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
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token does not exist." };
                    return;
                }
                else if (principal.Token.Expiration < DateTime.UtcNow)
                {
                    Trace.TraceInformation("Token is expired: based on cache.");
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token is expired." };
                    return;
                }
            }
            else
            {
                // Check the token, hard
                var result = TableStorage.Auth.Execute(TokenExtensions.GetExistingOperation(token));
                if (result.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    tokenCache.Add(token, false, defaultCachePolicy);
                    Trace.TraceWarning("Token does not exist: based on tablestorage.");
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token does not exist." };
                    return;
                }

                // Create principal
                var tokenEnt = (TokenEntity)result.Result;
                if (tokenEnt.Expiration < DateTime.UtcNow)
                {
                    Trace.TraceInformation("Token is expired: based on tablestorage.");
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token is expired." };
                    return;
                }
                principal = new MyPrincipal(new GenericIdentity(tokenEnt.Username), null) { Token = tokenEnt };
            }

            // Cache the principal based off of the token
            tokenCache.Add(token, principal, new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(principal.Token.Expiration) });

            // Stamp the current user
            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;
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