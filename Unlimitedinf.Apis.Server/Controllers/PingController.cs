using Microsoft.AspNetCore.Mvc;

namespace Unlimitedinf.Apis.Server.Controllers
{
    [Route("")]
    [Route("ping")]
    public class PingController : Controller
    {
        [HttpGet, HttpHead, HttpOptions]
        public IActionResult Ping()
        {
            return this.Ok();
        }
    }
}