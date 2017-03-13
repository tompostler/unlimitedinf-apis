using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Models.Versions;

namespace Unlimitedinf.Apis.Controllers.v1.Versions
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("version/accounts")]
    public class AccountsController : ApiController
    {
        [Route, HttpPost]
        public async Task<IHttpActionResult> RegisterNewUser(AccountApi account)
        {
            var checkIfExists = TableOperation.Retrieve<AccountEntity>(AccountValidator.PartitionKey, account.username.ToLowerInvariant());
            var result = await TableStorage.Version.ExecuteAsync(checkIfExists);
            if (result.Result != null)
                return Conflict();

            var insertNewAccount = TableOperation.Insert((AccountEntity)account, true);
            result = await TableStorage.Version.ExecuteAsync(insertNewAccount);

            return Content((HttpStatusCode)result.HttpStatusCode, (AccountApi)(AccountEntity)result.Result);
        }
    }
}