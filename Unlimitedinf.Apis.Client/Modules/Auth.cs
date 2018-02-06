using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Modules
{
    internal static class Auth
    {
        public static async Task Run(Options.Options.ModuleOptions options)
        {
            var opts = options as Options.Options.AuthOptions;

            switch (opts.Account)
            {
                case Options.CRUD.Create:
                    var accc = Input.GetAndValidate<Account>();
                    Log.Inf(await StaticHttpCommunicator.Post<Account>(Curl.AuAccount, accc));
                    break;

                case Options.CRUD.Update:
                    var usernamec = Input.Get("username");
                    var accu = Input.GetAndValidate<AccountUpdate>();
                    Log.Inf(await StaticHttpCommunicator.Put<Account>($"{Curl.AuAccount}/{usernamec}", accu));
                    break;

                case Options.CRUD.Delete:
                    var usernamed = Input.Get("username");
                    var accd = Input.GetAndValidate<AccountDelete>();
                    Log.Inf(await StaticHttpCommunicator.Delete<Account>($"{Curl.AuAccount}/{usernamed}", accd));
                    break;
            }

            if (!string.IsNullOrWhiteSpace(opts.ReadAccount))
                Log.Inf(await StaticHttpCommunicator.Get<Account>($"{Curl.AuAccount}/{opts.ReadAccount}"));

            if (opts.CreateToken)
            {
                var tok = Input.GetAndValidate<TokenCreate>();
                Log.Inf(await StaticHttpCommunicator.Post<Token>(Curl.AuToken, tok));
            }

            if (!string.IsNullOrWhiteSpace(opts.DeleteToken))
                Log.Inf(await StaticHttpCommunicator.Delete<Token>($"{Curl.AuToken}/{opts.DeleteToken}"));
        }
    }
}
