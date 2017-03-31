using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics;

namespace Unlimitedinf.Apis
{
    public class AiTelemetryConfig : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Properties["AppVersion"] = FileVersionInfo.GetVersionInfo(typeof(AiTelemetryConfig).Assembly.Location).FileVersion;
        }
    }
}