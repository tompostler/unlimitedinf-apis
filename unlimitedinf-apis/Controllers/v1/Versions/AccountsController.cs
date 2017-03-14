using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Models.Versions;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Controllers.v1.Versions
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("version/accounts")]
    public class AccountsController : ApiController
    {
        [Route, HttpPost]
        public async Task<IHttpActionResult> RegisterNewUser(AccountApi account)
        {
            var result = await TableStorage.Version.ExecuteAsync(account.GetExistingOperation());
            if (result.Result != null)
                return Conflict();

            var insertNewAccount = TableOperation.Insert((AccountEntity)account, true);
            result = await TableStorage.Version.ExecuteAsync(insertNewAccount);

            return Content((HttpStatusCode)result.HttpStatusCode, (AccountApi)(AccountEntity)result.Result);
        }

        [Route, HttpPut]
        public async Task<IHttpActionResult> UpdateAccountSecret(AccountApi account)
        {
            if (string.IsNullOrWhiteSpace(account.oldsecret))
                return BadRequest("account.oldsecret required.");

            var result = await TableStorage.Version.ExecuteAsync(account.GetExistingOperation());
            if (result.Result == null)
                return NotFound();

            if (!account.oldsecret.GetHashCodeSha512().Equals(((AccountEntity)result.Result).Secret))
                return StatusCode(HttpStatusCode.Forbidden);

            var upsertAccount = TableOperation.Replace((AccountEntity)account);
            result = await TableStorage.Version.ExecuteAsync(upsertAccount);

            return Content((HttpStatusCode)result.HttpStatusCode, (AccountApi)(AccountEntity)result.Result);
        }

        [Route, HttpDelete]
        public async Task<IHttpActionResult> DeleteAccountAndAllVersions(AccountApi account)
        {
            var result = await TableStorage.Version.ExecuteAsync(account.GetExistingOperation());
            if (result.Result == null)
                return NotFound();

            var entity = (AccountEntity)result.Result;
            if (!account.oldsecret.GetHashCodeSha512().Equals(entity.Secret))
                return StatusCode(HttpStatusCode.Forbidden);

            //TODO delete all versions for this account

            result = await TableStorage.Version.ExecuteAsync(TableOperation.Delete(entity));
            return Content((HttpStatusCode)result.HttpStatusCode, (AccountApi)(AccountEntity)result.Result);
        }
    }
}