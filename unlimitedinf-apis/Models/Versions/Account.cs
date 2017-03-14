using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Models.Versions
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

    public class AccountApi
    {
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.Username))]
        public string username { get; set; }

        [Required, EmailAddress]
        public string email { get; set; }

        [Required, StringLength(100)]
        public string secret { get; set; }

        [StringLength(100)]
        public string oldsecret { get; set; }

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

    public class AccountValidator
    {
        public const string PartitionKey = "accounts";

        public static ValidationResult Username(string username, ValidationContext context)
        {
            if (username != null && username.Equals(PartitionKey))
                return new ValidationResult($"Username cannot be '{PartitionKey}'");
            else
                return null;
        }
    }
}
