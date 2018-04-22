using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
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

        //public static OkObjectResult Ok(this Controller cont, object result)
        //{
        //    bool wantsHtml = false;

        //    if (cont.Request.Headers.ContainsKey("Accept"))
        //    {
        //        wantsHtml = cont.Request.Headers["Accept"].Contains("text/html");
        //    }

        //    if (!(result is string) && wantsHtml)
        //        // If the content is not a string and the accept types ask for text/html
        //        // (aka the browser is hitting this url)
        //        //  then pretty-print the json
        //        return cont.Ok(JsonConvert.SerializeObject(result, Formatting.Indented));

        //    return cont.Ok(result);
        //}
    }
}
