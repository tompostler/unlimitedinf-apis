using Newtonsoft.Json;
using System;
using System.IO;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client
{
    class Settings
    {
        private static Settings i;
        [JsonIgnore]
        public static Settings I
        {
            get
            {
                if (i == null)
                    if (SettingsFile.Exists)
                        i = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFile.FullName));
                    else
                        i = new Settings();
                return i;
            }
        }

        public string Username { get; set; }
        public string Token { get; set; }
        public bool VerboseInput { get; set; }

        private static readonly FileInfo SettingsFile = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UnlimitedInf", "Apis", "client.config"));

        public static void Save()
        {
            if (!SettingsFile.Exists)
                Directory.CreateDirectory(SettingsFile.Directory.FullName);
            File.WriteAllText(SettingsFile.FullName, JsonConvert.SerializeObject(I));
            Log.Ver("Saved configuration.");
        }
    }
}
