using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Unlimitedinf.Apis.Models.Versions
{
    public class AccountEntity : TableEntity
    { 
        [JsonIgnore]
        public string Username
        {
            get
            {
                return this.RowKey;
            }
            set
            {
                this.RowKey = value;
            }
        }

        public string Email { get; set; }

        public AccountEntity()
        {
            this.PartitionKey = "Users";
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

        public static implicit operator AccountEntity(AccountApi api)
        {
            return new AccountEntity
            {
                Username = api.username,
                Email = api.email
            };
        }
    }

    public class AccountValidator
    {
        public static ValidationResult Username(string username, ValidationContext context)
        {
            if (username.Equals("Users"))
                return new ValidationResult("Username cannot be 'Users'");
            else
                return null;
        }
    }
}
