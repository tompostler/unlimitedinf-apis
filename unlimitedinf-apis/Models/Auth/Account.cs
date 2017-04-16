using Microsoft.WindowsAzure.Storage.Table;
using System;
using Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis.Models.Auth
{
    public class AccountEntity : TableEntity
    {
        public string Username
        {
            get
            {
                return this.RowKey;
            }
            set
            {
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

        public static TableOperation GetExistingOperation(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException();

            return TableOperation.Retrieve<AccountEntity>(
                PartitionKey,
                username.ToLowerInvariant());
        }
    }
}