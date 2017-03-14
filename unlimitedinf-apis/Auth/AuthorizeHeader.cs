using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using Unlimitedinf.Apis.Models.Versions;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Auth
{
    public static class AuthorizeHeader
    {
        public static string Check(HttpActionContext actionContext, AccountEntity accountEntity)
        {
            // Get the authorization header
            string authHeader = GetAuthorizationHeader(actionContext);
            if (string.IsNullOrWhiteSpace(authHeader))
                return "Authorization header is empty.";

            // Check the authorization header (see below lines for format; Secret is SHA512 of original secret in upper case)
            string expected = $"{accountEntity.Username}{accountEntity.Secret}{DateTime.UtcNow.ToString("yyMMdd")}".GetHashCodeSha512();
            if (!expected.Equals(authHeader, StringComparison.InvariantCultureIgnoreCase))
                return "Authorization failed.";

            return null;
        }

        public static async Task<string> Check(HttpActionContext actionContext, string username)
        {
            // Get the account
            var result = await TableStorage.Version.ExecuteAsync(new AccountApi { username = username }.GetExistingOperation());
            if (result.Result == null)
                return "Account not found.";
            var accountEntity = (AccountEntity)result.Result;

            return Check(actionContext, accountEntity);
        }

        public static string GetAuthorizationHeader(HttpActionContext actionContext)
        {
            return actionContext.Request.Headers.Authorization.Scheme;
        }
    }
}