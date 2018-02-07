using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Axioms;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Modules
{
    internal static class Axiom
    {
        public static async Task Run(Options.Options.ModuleOptions options)
        {
            var opts = options as Options.Options.AxiomOptions;

            if (opts.Http.HasValue)
                Log.Inf(await StaticHttpCommunicator.Get<AxiomBase>($"{Curl.Axiom}/http/{opts.Http.Value}"));

            if (!string.IsNullOrWhiteSpace(opts.Type))
                Log.Inf(await StaticHttpCommunicator.Get<AxiomBase>($"{Curl.Axiom}/{opts.Type}/{opts.Id}"));
        }
    }
}
