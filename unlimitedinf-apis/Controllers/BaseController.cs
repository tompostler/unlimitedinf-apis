using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace Unlimitedinf.Apis.Controllers
{
    public class BaseController : ApiController
    {
        protected internal IHttpActionResult Ok(object content)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            bool wantsHtml = false, wantsJson = false;

            if (HttpContext.Current.Request.AcceptTypes != null)
            {
                wantsHtml = HttpContext.Current.Request.AcceptTypes.Contains("text/html");
                wantsJson = HttpContext.Current.Request.AcceptTypes.Contains("application/json");
            }

            if (!(content is string) && wantsHtml)
                // If the content is not a string and the accept types ask for text/html
                // (aka the browser is hitting this url)
                //  then pretty-print the json
                result.Content = new StringContent(JsonConvert.SerializeObject(content, Formatting.Indented), Encoding.UTF8, "application/json");
            else if (!(content is string) || wantsJson)
                // If the content is not a string or the accept types asks for application/json
                //  then return application/json
                result.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            else
                // Else return the string as text/plain
                result.Content = new StringContent(content as string);

            return new ResponseMessageResult(result);
        }
    }
}