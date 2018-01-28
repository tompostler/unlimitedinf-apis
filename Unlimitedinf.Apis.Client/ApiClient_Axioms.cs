using System.Threading.Tasks;
using CA = Unlimitedinf.Apis.Contracts.Axioms;

namespace Unlimitedinf.Apis.Client
{
    /// <summary>
    /// Axiom client.
    /// </summary>
    public sealed class ApiClient_Axioms
    {
        private ApiClient_Axioms() { }
        


        /// <summary>
        /// Read an axiom.
        /// </summary>
        public static async Task<CA.AxiomBase> AxiomRead(string type, string id)
        {
            return await StaticHttpCommunicator.Get<CA.AxiomBase>($"{Curl.AxBase}/{type}/{id}");
        }
    }
}
