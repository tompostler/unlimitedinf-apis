using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Web.Http.Versioning;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Unlimitedinf.Apis
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Make JSON the default response type
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            // API versioning and custom routing
            config.MapHttpAttributeRoutes();
            config.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
                o.ReportApiVersions = true;
            });

            // Default routing
            config.Routes.MapHttpRoute(
                name: "AllApis",
                routeTemplate: "api/{controller}"
            );

            // Custom ApplicationInsights things
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new AiTelemetryConfig());
            config.Services.Add(typeof(IExceptionLogger), new AiExceptionConfig());
        }
    }
}
