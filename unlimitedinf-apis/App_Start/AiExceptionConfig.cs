using Microsoft.ApplicationInsights;
using System.Web.Http.ExceptionHandling;

namespace Unlimitedinf.Apis
{
    public class AiExceptionConfig : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context?.Exception != null)
            {
                new TelemetryClient().TrackException(context.Exception);
            }
            base.Log(context);
        }
    }
}