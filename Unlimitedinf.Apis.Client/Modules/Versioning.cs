using System.Collections.Generic;
using System.Threading.Tasks;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Modules
{
    internal static class Versioning
    {
        public static async Task Run(Options.Options.ModuleOptions options)
        {
            var opts = options as Options.Options.VersioningOptions;
            HttpCommunicator c = null;
            if (opts.Count != Options.CRUD.Nothing || opts.Version != Options.CRUD.Nothing)
                c = new HttpCommunicator(Input.GetToken(true));

            switch (opts.Version)
            {
                case Options.CRUD.Create:
                    var verc = Input.GetAndValidate<Contracts.Versioning.Version>();
                    Log.Inf(await c.Post<Contracts.Versioning.Version>(Curl.VeVersion, verc));
                    break;

                case Options.CRUD.Update:
                    Log.Inf(await c.Patch<Contracts.Versioning.Version>(
                        $"{Curl.VeVersion}/{Input.Get("username")}/{Input.Get("versionName")}",
                        Input.GetAndValidate<Contracts.Versioning.VersionIncrement>()));
                    break;

                case Options.CRUD.Delete:
                    Log.Inf(await c.Delete<Contracts.Versioning.Version>($"{Curl.VeVersion}/{Input.Get("username")}/{Input.Get("versionName")}"));
                    break;
            }

            switch (opts.Count)
            {
                case Options.CRUD.Create:
                    var couc = Input.GetAndValidate<Contracts.Versioning.Count>();
                    Log.Inf(await c.Post<Contracts.Versioning.Count>(Curl.VeCount, couc));
                    break;

                case Options.CRUD.Update:
                    var couu = Input.GetAndValidate<Contracts.Versioning.CountChange>();
                    Log.Inf(await c.Patch<Contracts.Versioning.Count>($"{Curl.VeCount}/{Input.Get("username")}/{Input.Get("countName")}", couu));
                    break;

                case Options.CRUD.Delete:
                    Log.Inf(await c.Delete<Contracts.Versioning.Count>($"{Curl.VeCount}/{Input.Get("username")}/{Input.Get("countName")}"));
                    break;
            }

            if (!string.IsNullOrWhiteSpace(opts.ReadVersion))
                if (opts.ReadVersion.Contains("/"))
                    Log.Inf(await StaticHttpCommunicator.Get<Contracts.Versioning.Version>($"{Curl.VeVersion}/{opts.ReadVersion}"));
                else
                    Log.Inf(await StaticHttpCommunicator.Get<List<Contracts.Versioning.Version>>($"{Curl.VeVersion}/{opts.ReadVersion}"));

            if (!string.IsNullOrWhiteSpace(opts.ReadCount))
                if (opts.ReadCount.Contains("/"))
                    Log.Inf(await StaticHttpCommunicator.Get<Contracts.Versioning.Count>($"{Curl.VeCount}/{opts.ReadCount}"));
                else
                    Log.Inf(await StaticHttpCommunicator.Get<List<Contracts.Versioning.Count>>($"{Curl.VeCount}/{opts.ReadCount}"));
        }
    }
}
