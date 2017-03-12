using System.Net.Http;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Util;

namespace Unlimitedinf.Apis.Controllers
{
    [RoutePrefix("teapot")]
    public class TeapotController : ApiController
    {
        [Route, HttpDelete, HttpGet, HttpHead, HttpOptions, HttpPost, HttpPut]
        public IHttpActionResult ImATeapot()
        {
            return this.ResponseMessage(Request.CreateErrorResponse(MoreHttpStatusCodes.ImATeapot, new HttpError("I'm a teapot!")));
        }
    }
}