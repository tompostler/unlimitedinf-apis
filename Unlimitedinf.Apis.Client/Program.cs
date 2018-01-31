using Newtonsoft.Json;
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

            switch (Options.Options.Parse(args))
            {
                case Options.Module.Help:
                    Log.Inf(Options.Options.BaseHelpText);
                    return;
            }

            Log.Ver("Settings: " + JsonConvert.SerializeObject(Settings.I));
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore };
        }
    }
}
