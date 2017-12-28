using Microsoft.Web.Http;
using System;
using System.Web.Http;

namespace Unlimitedinf.Apis.Controllers.v1
{
    [ApiVersion("1.0")]
    [RoutePrefix("datetime")]
    public class DateTimesController : BaseController
    {
        [Route("yea"), HttpGet]
        public IHttpActionResult GetCurrentYear()
        {
            return Ok(DateTimeOffset.UtcNow.Year.ToString());
        }

        [Route("mon"), HttpGet]
        public IHttpActionResult GetCurrentMonth()
        {
            return Ok(DateTimeOffset.UtcNow.Month.ToString());
        }

        [Route("dom"), HttpGet]
        public IHttpActionResult GetCurrentDayOfMonth()
        {
            return Ok(DateTimeOffset.UtcNow.Day.ToString());
        }

        [Route("doy"), HttpGet]
        public IHttpActionResult GetCurrentDayOfYear()
        {
            return Ok(DateTimeOffset.UtcNow.DayOfYear.ToString());
        }

        [Route("dow"), HttpGet]
        public IHttpActionResult GetCurrentDayOfWeek()
        {
            return Ok(((int)DateTimeOffset.UtcNow.DayOfWeek).ToString());
        }

        [Route("hou"), HttpGet]
        public IHttpActionResult GetCurrentHour()
        {
            return Ok(DateTimeOffset.UtcNow.Hour.ToString());
        }

        [Route("min"), HttpGet]
        public IHttpActionResult GetCurrentMinute()
        {
            return Ok(DateTimeOffset.UtcNow.Minute.ToString());
        }

        [Route("sec"), HttpGet]
        public IHttpActionResult GetCurrentSecond()
        {
            return Ok(DateTimeOffset.UtcNow.Second.ToString());
        }

        [Route("fff"), HttpGet]
        public IHttpActionResult GetCurrentFractionalSeconds()
        {
            return Ok(DateTimeOffset.UtcNow.Millisecond.ToString());
        }

        [Route("iso"), HttpGet]
        public IHttpActionResult GetCurrentIso()
        {
            return Ok(DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
    }
}