using System.Web.Http;

namespace Unlimitedinf.Apis.Controllers
{
    [RoutePrefix("ping")]
    public class PingController : ApiController
    {
        [Route, HttpGet, HttpHead, HttpOptions]
        public IHttpActionResult Ping()
        {
            return this.Ok();
        }
    }
}