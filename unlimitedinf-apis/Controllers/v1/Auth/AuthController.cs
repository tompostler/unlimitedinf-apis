using Microsoft.Web.Http;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;

namespace Unlimitedinf.Apis.Controllers.v1.Auth
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {
    }
}