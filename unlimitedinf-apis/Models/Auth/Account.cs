using Microsoft.WindowsAzure.Storage.Table;
using Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis.Models.Auth
{
    public class AccountEntity : TableEntity
    {
        public new string PartitionKey => AccountExtensions.PartitionKey;

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
                this.RowKey = value;
            }
        }

        public string Email { get; set; }

        public string Secret { get; set; }

        public AccountEntity() { }

        public AccountEntity(Account account)
        {
            this.Username = account.username;
            this.Email = account.email;
            this.Secret = account.secret;
        }

        public static implicit operator Account(AccountEntity entity)
        {
            return new Account
            {
                email = entity.Email,
                secret = entity.Secret,
                username = entity.Username
            };
        }
    }

    public static class AccountExtensions
    {
        public const string PartitionKey = "accounts";

        public static TableOperation GetExistingOperation(this Account account)
        {
            return TableOperation.Retrieve<AccountEntity>(
                PartitionKey,
                account.username.ToLowerInvariant());
        }
    }
}