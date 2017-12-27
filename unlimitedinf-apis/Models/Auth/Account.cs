using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis.Models.Auth
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

        public AccountEntity() { }

        public AccountEntity(Account account)
        {
            this.PartitionKey = AccountExtensions.PartitionKey;

            this.Username = account.username;
            this.Email = account.email;
            this.Secret = account.secret;
        }

        public static explicit operator Account(AccountEntity entity)
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
            return GetExistingOperation(account.username);
        }

        public static async Task<bool> Exists(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            var op = TableOperation.Retrieve(PartitionKey, username.ToLowerInvariant(), new List<string> { "RowKey" });
            var res = await TableStorage.Auth.ExecuteAsync(op);

            return res.Result != null;
        }

        public static TableOperation GetExistingOperation(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            return TableOperation.Retrieve<AccountEntity>(
                PartitionKey,
                username.ToLowerInvariant());
        }
    }
}