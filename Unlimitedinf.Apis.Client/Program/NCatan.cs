using Newtonsoft.Json;
using System;

namespace Unlimitedinf.Apis.Client.Program
{
    using Unlimitedinf.Apis.Contracts.Notes;
    using Unlimitedinf.Tools;

    internal static class NCatan
    {
        internal static int Run(string[] args)
        {
            if (args.Length == 0)
                return App.PrintHelp();

            var rargs = new string[args.Length - 1];
            Array.Copy(args, 1, rargs, 0, args.Length - 1);
            switch (args[0])
            {
                case "create+":
                    return NCatan.Create();
                case "read":
                    return NCatan.Read(rargs);
                case "reads":
                    return NCatan.ReadStats(rargs);
                case "delete":
                    return NCatan.Delete(rargs);

                default:
                    return App.PrintHelp();
            }
        }

        private static int Create()
        {
            Catan catan = new Catan();

            Console.Write("name: ");
            catan.name = Console.ReadLine().Trim();

            Console.Write("How often do you want to print the roll stats? [5]: ");
            int.TryParse(Console.ReadLine(), out int statFreq);
            if (statFreq <= 0)
                statFreq = 5;
            int statCount = 0;

            Console.WriteLine("Enter die rolls in the form of YR, where Y is the yellow die value and R is the red die value.");
            Console.WriteLine("A blank line will end the entry.");
            Console.WriteLine("An 's' will immediately print the stats and reset the counter.");
            var roll = Console.ReadLine().Trim();
            while (!string.IsNullOrWhiteSpace(roll))
            {
                if (roll.Equals("s", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(new CatanStats(catan));
                    statCount = 0;
                }
                else if (roll.Length == 2
                        && int.TryParse(roll[0].ToString(), out int y)
                        && y >= 1
                        && y <= 6
                        && int.TryParse(roll[0].ToString(), out int r)
                        && r >= 1
                        && r <= 6
                    )
                {
                    catan.rolls.Add(new Catan.Roll { y = y, r = r });
                    if (++statCount >= statFreq)
                    {
                        Console.WriteLine(new CatanStats(catan));
                        statCount = 0;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid. Try again.");
                }
                roll = Console.ReadLine().Trim();
            }

            string token = Input.GetToken();
            catan.username = Contracts.Auth.Token.GetUsernameFrom(token);
            Input.Validate(catan, token);
            var client = new ApiClient(token);
            catan = client.Notes.CatanCreate(catan).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(catan, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Read(string[] args)
        {
            if (args.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(args[0]))
                {
                    Console.Error.WriteLine("Did not supply username argument.");
                    return ExitCode.ValidationFailed;
                }
                if (string.IsNullOrWhiteSpace(args[1]))
                {
                    Console.Error.WriteLine("Did not supply catanName argument.");
                    return ExitCode.ValidationFailed;
                }

                var result = ApiClient_Notes.CatanRead(args[0], args[1]).GetAwaiter().GetResult();
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }

            Log.Err("Unexpected arguments.");
            return ExitCode.ValidationFailed;
        }

        private static int ReadStats(string[] args)
        {
            if (args.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(args[0]))
                {
                    Console.Error.WriteLine("Did not supply username argument.");
                    return ExitCode.ValidationFailed;
                }
                if (string.IsNullOrWhiteSpace(args[1]))
                {
                    Console.Error.WriteLine("Did not supply catanName argument.");
                    return ExitCode.ValidationFailed;
                }

                var result = ApiClient_Notes.CatanReadStats(args[0], args[1]).GetAwaiter().GetResult();
                Console.WriteLine(result);
                return ExitCode.Success;
            }

            Log.Err("Unexpected arguments.");
            return ExitCode.ValidationFailed;
        }

        private static int Delete(string[] args)
        {
            if (args.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(args[0]))
                {
                    Log.Err("Did not supply catanName argument.");
                    return ExitCode.ValidationFailed;
                }

                var client = new ApiClient(Input.GetToken());

                var result = client.Notes.CatanDelete(args[0]).GetAwaiter().GetResult();
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }

            Log.Err("Did not supply catanName argument.");
            return ExitCode.ValidationFailed;
        }
    }
}
