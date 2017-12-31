using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Unlimitedinf.Apis.Server.Filters
{
    public class RequireHttpsNonDebugLocalhostAttribute : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsHttps)
                HandleNonHttpsRequest(filterContext);
        }

        protected override void HandleNonHttpsRequest(AuthorizationFilterContext filterContext)
        {
            if (filterContext.HttpContext.Request.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                return;

            base.HandleNonHttpsRequest(filterContext);
        }
    }
}
