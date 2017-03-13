using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web;
using System.Web.Http;

namespace Unlimitedinf.Apis
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
