using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Modules
{
    internal static class Catan
    {
        public static async Task Run(Options.Options.ModuleOptions options)
        {
            var opts = options as Options.Options.CatanOptions;
            HttpCommunicator c = null;
            if (opts.Catan != Options.CRUD.Nothing)
                c = new HttpCommunicator(Input.GetToken(true));

            switch (opts.Catan)
            {
                case Options.CRUD.Create:
                    Log.Inf(await c.Post<Contracts.Catan>(Curl.Catan, CreateCatanPlus()));
                    break;

                case Options.CRUD.Delete:
                    Log.Inf(await c.Delete<Contracts.Catan>($"{Curl.Catan}/{Input.Get("catanName")}"));
                    break;
            }

            if (!string.IsNullOrWhiteSpace(opts.ReadCatan))
                if (opts.ReadCatan.Contains("/"))
                    Log.Inf(await StaticHttpCommunicator.Get<Contracts.Catan>($"{Curl.Catan}/{opts.ReadCatan}"));
                else
                    Log.Inf(await StaticHttpCommunicator.Get<List<Contracts.Catan>>($"{Curl.Catan}/{opts.ReadCatan}"));

            if (!string.IsNullOrWhiteSpace(opts.ReadCatanStats))
                Log.Inf(await StaticHttpCommunicator.Get<string>($"{Curl.Catan}/{opts.ReadCatanStats}/stats"));
        }

        private static Contracts.Catan CreateCatanPlus()
        {
            Contracts.Catan catan = new Contracts.Catan();

            catan.username = Input.Get("username");
            catan.name = Input.Get("name");
            int.TryParse(Input.Get("How often do you want to print the roll stats? [5]"), out int statFreq);
            if (statFreq <= 0)
                statFreq = 5;
            int statCount = 0;

            Log.Inf("Enter die rolls in the form of RY, where R is the red die value and Y is the yellow die value.");
            Log.Inf("A blank line will end the entry.");
            Log.Inf("An 's' will immediately print the stats and reset the counter.");
            var roll = Console.ReadLine().Trim();
            while (!string.IsNullOrWhiteSpace(roll))
            {
                if (roll.Equals("s", StringComparison.OrdinalIgnoreCase))
                {
                    Log.Inf(new Contracts.CatanStats(catan).ToString());
                    statCount = 0;
                }
                else if (roll.Length == 2
                        && int.TryParse(roll[0].ToString(), out int r)
                        && r >= 1
                        && r <= 6
                        && int.TryParse(roll[1].ToString(), out int y)
                        && y >= 1
                        && y <= 6
                    )
                {
                    catan.rolls.Add(new Contracts.Catan.Roll { r = r, y = y });
                    if (++statCount >= statFreq)
                    {
                        Log.Inf(new Contracts.CatanStats(catan).ToString());
                        statCount = 0;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid. Try again.");
                }
                roll = Console.ReadLine().Trim();
            }

            Log.Inf("Dumping the catan object as json just in case posting goes poorly:");
            Log.Inf(JsonConvert.SerializeObject(catan, Formatting.None));

            return catan;
        }
    }
}
