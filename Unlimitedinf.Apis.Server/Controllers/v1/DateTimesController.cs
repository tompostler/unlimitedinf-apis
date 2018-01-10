using Microsoft.AspNetCore.Mvc;
using System;

namespace Unlimitedinf.Apis.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("datetime")]
    public class DateTimesController : Controller
    {
        [HttpGet("yea")]
        public IActionResult GetCurrentYear()
        {
            return Ok(DateTimeOffset.UtcNow.Year.ToString());
        }

        [HttpGet("mon")]
        public IActionResult GetCurrentMonth()
        {
            return Ok(DateTimeOffset.UtcNow.Month.ToString());
        }

        [HttpGet("dom")]
        public IActionResult GetCurrentDayOfMonth()
        {
            return Ok(DateTimeOffset.UtcNow.Day.ToString());
        }

        [HttpGet("doy")]
        public IActionResult GetCurrentDayOfYear()
        {
            return Ok(DateTimeOffset.UtcNow.DayOfYear.ToString());
        }

        [HttpGet("dow")]
        public IActionResult GetCurrentDayOfWeek()
        {
            return Ok(((int)DateTimeOffset.UtcNow.DayOfWeek).ToString());
        }

        [HttpGet("hou")]
        public IActionResult GetCurrentHour()
        {
            return Ok(DateTimeOffset.UtcNow.Hour.ToString());
        }

        [HttpGet("min")]
        public IActionResult GetCurrentMinute()
        {
            return Ok(DateTimeOffset.UtcNow.Minute.ToString());
        }

        [HttpGet("sec")]
        public IActionResult GetCurrentSecond()
        {
            return Ok(DateTimeOffset.UtcNow.Second.ToString());
        }

        [HttpGet("fff")]
        public IActionResult GetCurrentFractionalSeconds()
        {
            return Ok(DateTimeOffset.UtcNow.Millisecond.ToString());
        }

        [HttpGet("iso")]
        public IActionResult GetCurrentIso()
        {
            return Ok(DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
    }
}