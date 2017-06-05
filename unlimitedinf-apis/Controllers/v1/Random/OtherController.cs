using Microsoft.Web.Http;
using System;
using System.Web.Http;

namespace Unlimitedinf.Apis.Controllers.v1.Random
{
    [ApiVersion("1.0")]
    [RoutePrefix("random")]
    public class OtherController : ApiController
    {
        [Route("guid"), HttpGet]
        public IHttpActionResult GuidMethod()
        {
            return Ok(Guid.NewGuid().ToString("D"));
        }
    }
}