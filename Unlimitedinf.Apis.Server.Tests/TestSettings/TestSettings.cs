using Newtonsoft.Json;
using System;
using System.IO;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public sealed class S
    {
        public static S I { get; }

        static S()
        {
            I = JsonConvert.DeserializeObject<S>(
                File.ReadAllText(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "TestSettings",
                        "testsettings.json")));
        }

        public string BaseUri { get; set; }
    }
}
