using Newtonsoft.Json;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client
{
    static class Program
    {
        public static void Main(string[] args)
        {
            Log.PrintVerbosityLevel = false;

            // TODO: Parse options here

            Log.Ver("Settings: " + JsonConvert.SerializeObject(Settings.I));
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore };
        }
    }
}
