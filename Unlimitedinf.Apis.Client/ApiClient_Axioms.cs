using System.Threading.Tasks;
using CA = Unlimitedinf.Apis.Contracts.Axioms;

namespace Unlimitedinf.Apis.Client
{
    public sealed class ApiClient_Axioms
    {
        private ApiClient_Axioms() { }
        


        public static async Task<CA.AxiomBase> AxiomRead(string type, string id)
        {
            return await StaticHttpCommunicator.Get<CA.AxiomBase>(Curl.AxBase + $"?type={type}&id={id}");
        }
    }
}
