using Newtonsoft.Json;
using System;
using Unlimitedinf.Apis.Contracts;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Program
{
    internal static class RepoProgram
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
                    return RepoProgram.Create(rargs);
                case "read":
                    return RepoProgram.Read();
                case "readps":
                    return RepoProgram.ReadPsScript();
                case "update":
                    return RepoProgram.Update(rargs);
                case "delete":
                    return RepoProgram.Delete(rargs);

                default:
                    return App.PrintHelp();
            }
        }

        private static int Create(string[] args)
        {
            Repo repo = null;
            string token = null;
            if (args.Length == 1)
                repo = args[0].Deserialize<Repo>(out token);
            else
                repo = Input.Get<Repo>(out token);
            Input.Validate(repo, token);

            var client = new ApiClient(token);

            repo = client.Repos.Create(repo).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(repo, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Read()
        {
            var client = new ApiClient(Input.GetToken());

            var result = client.Repos.Read().GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(result, Formatting.Indented));
            return ExitCode.Success;
        }

        private static int ReadPsScript()
        {
            var client = new ApiClient(Input.GetToken());

            var result = client.Repos.ReadPsScript().GetAwaiter().GetResult();
            Log.Inf(result);
            return ExitCode.Success;
        }

        private static int Update(string[] args)
        {
            Repo repo = null;
            string token = null;
            if (args.Length == 1)
                repo = args[0].Deserialize<Repo>(out token);
            else
                repo = Input.Get<Repo>(out token);
            Input.Validate(repo, token);

            var client = new ApiClient(token);

            var version = client.Repos.Update(repo).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(version, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Delete(string[] args)
        {
            if (args.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(args[0]))
                {
                    Log.Err("Did not supply reponame argument.");
                    return ExitCode.ValidationFailed;
                }

                var client = new ApiClient(Input.GetToken());

                var result = client.Repos.Delete(args[0]).GetAwaiter().GetResult();
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }

            Log.Err("Did not supply reponame argument.");
            return ExitCode.ValidationFailed;
        }
    }
}
