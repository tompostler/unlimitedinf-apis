using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Unlimitedinf.Apis
{
    public class AiTelemetryConfig : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Properties["AppVersion"] = typeof(AiTelemetryConfig).Assembly.GetName().Version.ToString();
        }
    }
}