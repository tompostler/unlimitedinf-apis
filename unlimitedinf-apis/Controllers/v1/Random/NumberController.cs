using Microsoft.Web.Http;
using System;
using System.Security.Cryptography;
using System.Web.Http;

namespace Unlimitedinf.Apis.Controllers.v1.Random
{
    [ApiVersion("1.0")]
    [RoutePrefix("random/number")]
    public class NumberController : ApiController
    {
        private static RNGCryptoServiceProvider _rngCsp = new RNGCryptoServiceProvider();

        [Route, HttpGet]
        public IHttpActionResult Default(long min = 0, long max = 101)
        {
            // Standard inclusive lower bound, exclusive upper bound. Defaults to [0,100]
            if (min >= max)
                return BadRequest("max must be greater than min");

            var bytes = new byte[8];
            _rngCsp.GetBytes(bytes);
            long val = 0;
            for (int i = 0; i < 8; i++)
                val = val + (bytes[i] << i * 8);
            val = Math.Abs(val);

            val %= max - min;
            val += min;

            return Ok(val);
        }
    }
}