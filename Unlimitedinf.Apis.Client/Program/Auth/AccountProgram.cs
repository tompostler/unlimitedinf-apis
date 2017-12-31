using Newtonsoft.Json;
using System;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Program.Auth
{
    internal static class AccountProgram
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
                    return AccountProgram.Create(rargs);
                case "read":
                    return AccountProgram.Read(rargs);
                case "update":
                    return AccountProgram.Update(rargs);
                case "delete":
                    return AccountProgram.Delete(rargs);

                default:
                    return App.PrintHelp();
            }
        }

        private static int Create(string[] args)
        {
            Account account = null;
            if (args.Length == 1)
                account = JsonConvert.DeserializeObject<Account>(args[0]);
            else
                account = Input.Get<Account>();
            Input.Validate(account);

            account = ApiClientAuth.Account.Create(account).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(account, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Read(string[] args)
        {
            if (args.Length != 1 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.Error.WriteLine("Did not supply username argument.");
                return ExitCode.ValidationFailed;
            }

            var account = ApiClientAuth.Account.Read(args[0]).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(account, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Update(string[] args)
        {
            AccountUpdate account = null;
            if (args.Length == 1)
                account = JsonConvert.DeserializeObject<AccountUpdate>(args[0]);
            else
                account = Input.Get<AccountUpdate>();
            Input.Validate(account);

            ApiClientAuth.Account.Update(account).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(new AccountUpdate { email = account.email }, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Delete(string[] args)
        {
            Account account = null;
            if (args.Length == 1)
                account = JsonConvert.DeserializeObject<Account>(args[0]);
            else
                account = Input.Get<Account>();
            Input.Validate(account);

            ApiClientAuth.Account.Delete(account).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(new Account { email = account.email, username = account.username }, Formatting.Indented));

            return ExitCode.Success;
        }
    }
}
