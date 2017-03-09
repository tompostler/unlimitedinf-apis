using Microsoft.Web.Http.Versioning;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Unlimitedinf.Apis
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Make JSON the default response type
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            config.MapHttpAttributeRoutes();
            config.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
                o.ReportApiVersions = true;
            });

            config.Routes.MapHttpRoute(
                name: "AllApis",
                routeTemplate: "api/{controller}"
            );
        }
    }
}
