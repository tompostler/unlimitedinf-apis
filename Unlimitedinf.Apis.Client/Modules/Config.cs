using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Modules
{
    internal static class Config
    {
        public static void Run(Options.Options.ModuleOptions options)
        {
            var opts = options as Options.Options.ConfigOptions;
            if (opts.Show)
            {
                Log.Inf("Existing configuration:");
                Log.Inf(Settings.I);
            }

            if (opts.Clear)
            {
                Settings.I.Username = null;
                Settings.I.Token = null;
                Settings.Save();
                Log.Inf("Cleared configuration.");
            }

            if (opts.Username != null)
            {
                Settings.I.Username = opts.Username;
                Settings.Save();
                Log.Inf("Updated username.");
            }

            if (opts.Token != null)
            {
                Settings.I.Token = opts.Token;
                Settings.Save();
                Log.Inf("Updated token.");

                if (Token.IsTokenExpired(opts.Token))
                    Log.Wrn("However, token is not valid.");
            }
        }
    }
}
