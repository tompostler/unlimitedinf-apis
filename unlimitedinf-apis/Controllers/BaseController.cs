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

            if (!(content is string) || HttpContext.Current.Request.AcceptTypes.Contains("application/json"))
                result.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            else
                result.Content = new StringContent(content as string);

            return new ResponseMessageResult(result);
        }
    }
}