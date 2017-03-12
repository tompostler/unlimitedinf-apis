using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;

namespace Unlimitedinf.Apis.Controllers.v1.Versions
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("version/accounts")]
    public class AccountsController : ApiController
    {
        [Route, HttpPost, ValidateViewModel]
        public IHttpActionResult RegisterNewUser(Models.Versions.AccountApi account)
        {
            return Ok();
        }
    }
}