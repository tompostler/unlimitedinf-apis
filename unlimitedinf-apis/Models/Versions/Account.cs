using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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

        public AccountEntity()
        {
            this.PartitionKey = "Users";
        }

        public static implicit operator AccountApi(AccountEntity entity)
        {
            return new AccountApi
            {
                username = entity.Username
            };
        }
    }

    public class AccountApi
    {
        [StringLength(1000), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.Validate))]
        public string username { get; set; }

        public static implicit operator AccountEntity(AccountApi api)
        {
            return new AccountEntity
            {
                Username = api.username
            };
        }
    }

    public class AccountValidator
    {
        public static ValidationResult Validate(string username, ValidationContext context)
        {
            if (username.Equals("Users"))
                return new ValidationResult("Username cannot be 'Users'");
            else
                return null;
        }
    }
}
