using Microsoft.WindowsAzure.Storage.Table;
using Unlimitedinf.Apis.Contracts.Version;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Models.Version
{
    public class AccountEntity : TableEntity
    {
        private string _Username;
        public string Username
        {
            get
            {
                return this._Username;
            }
            set
            {
                this._Username = value;
                this.RowKey = value.ToLowerInvariant();
            }
        }

        public string Email { get; set; }

        public string Secret { get; set; }

        public AccountEntity()
        {
            this.PartitionKey = AccountValidator.PartitionKey;
        }

        public static implicit operator AccountApi(AccountEntity entity)
        {
            return new AccountApi
            {
                username = entity.Username,
                email = entity.Email
            };
        }
    }

    public class AccountApi : Account
    {
        public static implicit operator AccountEntity(AccountApi api)
        {
            return new AccountEntity
            {
                Username = api.username,
                Email = api.email,
                Secret = api.secret.GetHashCodeSha512()
            };
        }

        public TableOperation GetExistingOperation()
        {
            return TableOperation.Retrieve<AccountEntity>(AccountValidator.PartitionKey, this.username.ToLowerInvariant());
        }
    }
}
