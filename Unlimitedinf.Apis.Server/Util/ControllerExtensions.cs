using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Unlimitedinf.Apis.Server.Util
{
    public static class ControllerExtensions
    {
        public static IActionResult TableResultStatus<TApi>(this Controller cont, int statusCode, TApi resultObject)
        {
            var returnCode = (HttpStatusCode)statusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return cont.StatusCode((int)returnCode, resultObject);
        }

        public static bool IsBadUsername(this Controller cont, string username)
        {
            return !cont.User.Identity.Name.Equals(username, StringComparison.OrdinalIgnoreCase);
        }
    }
}
