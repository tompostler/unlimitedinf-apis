using Microsoft.AspNetCore.Mvc;
using Unlimitedinf.Apis.Server.Util;

namespace Unlimitedinf.Apis.Server.Controllers
{
    [Route("teapot")]
    public class TeapotController : Controller
    {
        [HttpDelete, HttpGet, HttpHead, HttpOptions, HttpPost, HttpPut]
        public IActionResult ImATeapot()
        {
            return this.StatusCode(MoreHttpStatusCodes.ImATeapot, "I'm a teapot!");
        }
    }
}