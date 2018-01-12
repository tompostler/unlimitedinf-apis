using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Server.Filters;
using Unlimitedinf.Apis.Server.Models.Axioms;

namespace Unlimitedinf.Apis.Server.Controllers.v1
{
    [RequireHttpsNonLocalhostAttribute, ApiVersion("1.0")]
    [Route("axioms")]
    public class AxiomsController : Controller
    {
        [HttpGet("http/{id}")]
        public async Task<IActionResult> GetHttp(string id)
        {
            var axiom = await AxiomCache.Get(HttpAxiomEntity.PartitionKeyValue, id);

            if (axiom == null)
                return NotFound();
            return Ok(axiom);
        }

        [HttpGet("{type}/{id}")]
        public async Task<IActionResult> GetRandomAxiom(string type, string id)
        {
            var axiom = await AxiomCache.Get(type, id);

            if (axiom == null)
                return NotFound();
            return Ok(axiom);
        }
    }
}
