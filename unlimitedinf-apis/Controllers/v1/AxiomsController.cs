using Microsoft.Web.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Models.Axioms;

namespace Unlimitedinf.Apis.Controllers.v1
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("axioms")]
    public class AxiomsController : BaseController
    {
        [Route("http/{id}"), HttpGet]
        public async Task<IHttpActionResult> GetHttp(string id)
        {
            var axiom = await AxiomCache.Get(HttpAxiomEntity.PartitionKeyValue, id);

            if (axiom == null)
                return NotFound();
            return Ok(axiom);
        }

        [Route("{type}/{id}"), Route, HttpGet]
        public async Task<IHttpActionResult> GetRandomAxiom(string type, string id)
        {
            var axiom = await AxiomCache.Get(type, id);

            if (axiom == null)
                return NotFound();
            return Ok(axiom);
        }
    }
}
