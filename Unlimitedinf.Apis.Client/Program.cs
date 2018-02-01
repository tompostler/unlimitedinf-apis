using Newtonsoft.Json;
using System;
using Unlimitedinf.Apis.Client.Options;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client
{
    static class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            Log.Verbosity = Log.VerbositySetting.Verbose;
#endif // DEBUG
            Log.PrintVerbosityLevel = false;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore };
            Log.Ver("CONFIG: " + JsonConvert.SerializeObject(Settings.I));

            try
            {
                (var module, var options) = Options.Options.Parse(args);
                switch (module)
                {
                    case Module.Help:
                        Log.Inf(Options.Options.BaseHelpText);
                        return;
                    case Module.Config:
                        Modules.Config.Run(options);
                        return;
                    case Module.Auth:
                        throw new NotImplementedException();
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Err(e.ToString());
                U.Exit();
            }
        }
    }
}
