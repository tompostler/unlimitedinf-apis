using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Unlimitedinf.Apis.Server.Tests
{
    public sealed class TestSettings
    {
        public static TestSettings I { get; }

        static TestSettings()
        {
            I = JsonConvert.DeserializeObject<TestSettings>(
                File.ReadAllText(
                    Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        "TestSettings",
                        "testsettings.json")));
        }

        public Uri BaseUri { get; set; }
    }
}
