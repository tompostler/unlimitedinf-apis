﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Unlimitedinf.Apis.Auth
{
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
#if DEBUG
            if (!actionContext.Request.RequestUri.AbsoluteUri.StartsWith("http://localhost:62789/") && actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
#else
            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
#endif
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "HTTPS Required"
                };
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}