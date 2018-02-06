using System.Collections.Generic;
using System.Threading.Tasks;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Modules
{
    internal static class Repo
    {
        public static async Task Run(Options.Options.ModuleOptions options)
        {
            var opts = options as Options.Options.RepoOptions;
            HttpCommunicator c = new HttpCommunicator(Input.GetToken(true));

            switch (opts.Repo)
            {
                case Options.CRUD.Create:
                    var repc = Input.GetAndValidate<Contracts.Repo>();
                    Log.Inf(await c.Post<Contracts.Repo>(Curl.Repo, repc));
                    break;

                case Options.CRUD.Read:
                    Log.Inf(await c.Get<List<Contracts.Repo>>(Curl.Repo));
                    break;

                case Options.CRUD.Delete:
                    var name = Input.Get("name");
                    Log.Inf(await c.Delete<Contracts.Repo>($"{Curl.Repo}/{name}"));
                    break;
            }

            if (opts.Powershell)
                Log.Inf(await c.Get<string>($"{Curl.Repo}/ps-script"));
        }
    }
}
