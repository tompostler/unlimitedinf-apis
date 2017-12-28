using Newtonsoft.Json;
using System;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Program.Versioning
{
    using Unlimitedinf.Apis.Contracts.Versioning;

    internal static class VersionProgram
    {
        internal static int Run(string[] args)
        {
            if (args.Length == 0)
                return App.PrintHelp();

            var rargs = new string[args.Length - 1];
            Array.Copy(args, 1, rargs, 0, args.Length - 1);
            switch (args[0])
            {
                case "create":
                    return VersionProgram.Create(rargs);
                case "read":
                    return VersionProgram.Read(rargs);
                case "update":
                    return VersionProgram.Update(rargs);
                case "delete":
                    return VersionProgram.Delete(rargs);

                default:
                    return App.PrintHelp();
            }
        }

        private static int Create(string[] args)
        {
            Version version = null;
            string token = null;
            if (args.Length == 1)
                version = args[0].Deserialize<Version>(out token);
            else
                version = Input.Get<Version>(out token);
            Input.Validate(version, token);

            var client = new ApiClient(token);

            version = client.Versioning.VersionCreate(version).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(version, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Read(string[] args)
        {
            if (args.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(args[0]))
                {
                    Console.Error.WriteLine("Did not supply username argument.");
                    return ExitCode.ValidationFailed;
                }

                var result = ApiClient_Versioning.VersionRead(args[0]).GetAwaiter().GetResult();
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }

            if (args.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
                {
                    Console.Error.WriteLine("Did not supply username or versionName argument.");
                    return ExitCode.ValidationFailed;
                }

                var result = ApiClient_Versioning.VersionRead(args[0], args[1]).GetAwaiter().GetResult();
                Log.Inf(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }

            Log.Err("Unexpected arguments.");
            return ExitCode.ValidationFailed;
        }

        private static int Update(string[] args)
        {
            VersionIncrement versionInc = null;
            string token = null;
            if (args.Length == 1)
                versionInc = args[0].Deserialize<VersionIncrement>(out token);
            else
                versionInc = Input.Get<VersionIncrement>(out token);
            Input.Validate(versionInc, token);

            var client = new ApiClient(token);
            var version = client.Versioning.VersionUpdate(versionInc).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(version, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Delete(string[] args)
        {
            Version version = null;
            string token = null;
            if (args.Length == 1)
                version = args[0].Deserialize<Version>(out token);
            else
                version = Input.Get<Version>(out token);
            Input.Validate(version, token);

            var client = new ApiClient(token);

            version = client.Versioning.VersionDelete(version.username, version.name).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(version, Formatting.Indented));

            return ExitCode.Success;
        }
    }
}
