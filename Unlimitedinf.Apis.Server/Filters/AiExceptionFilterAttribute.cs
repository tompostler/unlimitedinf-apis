using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Unlimitedinf.Apis.Server.Filters
{
    public class AiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            new TelemetryClient().TrackException(context.Exception);
        }
    }
}
