using Newtonsoft.Json;
using System;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Program
{
    internal static class AuToken
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
                    return AuToken.Create(rargs);
                case "delete":
                    return AuToken.Delete(rargs);
                case "save":
                    return AuToken.Save(rargs);

                default:
                    return App.PrintHelp();
            }
        }

        private static int Create(string[] args)
        {
            TokenCreate tokenc = null;
            if (args.Length == 1)
                tokenc = JsonConvert.DeserializeObject<TokenCreate>(args[0]);
            else
                tokenc = Input.Get<TokenCreate>();
            Input.Validate(tokenc);

            var token = ApiClientAuth.Token.Create(tokenc).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(token, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Delete(string[] args)
        {
            TokenDelete token = null;
            if (args.Length == 1)
                token = JsonConvert.DeserializeObject<TokenDelete>(args[0]);
            else
                token = Input.Get<TokenDelete>();
            Input.Validate(token);

            ApiClientAuth.Token.Delete(token).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(new Token { name = token.name, token = token.token, username = token.username }, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Save(string[] args)
        {
            if (args.Length != 1)
            {
                Log.Err("You did not supply a token.");
                return ExitCode.ValidationFailed;
            }
            else if (Token.IsTokenExpired(args[0]))
            {
                Log.Err("You supplied an invalid or expired token.");
                return ExitCode.ValidationFailed;
            }

            Properties.Settings.Default.ApiToken = args[0];
            Properties.Settings.Default.Save();
            return ExitCode.Success;
        }
    }
}
