using Microsoft.WindowsAzure.Storage.Table;
using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Models.Versions
{
    public class VersionEntity : TableEntity
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
                this.PartitionKey = value.ToLowerInvariant();
            }
        }

        [IgnoreProperty]
        public string Name
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

        [IgnoreProperty]
        public SemVer Version { get; set; }

        public string TSVersion
        {
            get
            {
                return this.Version.ToString();
            }
            set
            {
                this.Version = SemVer.Parse(value);
            }
        }

        public static implicit operator VersionApi(VersionEntity entity)
        {
            return new VersionApi
            {
                username = entity.Username,
                name = entity.Name,
                version = entity.Version
            };
        }
    }

    public class VersionApi
    {
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.Username))]
        public string username { get; set; }

        [Required, StringLength(100)]
        public string name { get; set; }

        [Required]
        public SemVer version { get; set; }

        public static implicit operator VersionEntity(VersionApi api)
        {
            return new VersionEntity
            {
                Username = api.username,
                Name = api.name,
                Version = api.version
            };
        }

        public TableOperation GetExistingOperation()
        {
            return TableOperation.Retrieve<VersionEntity>(this.username.ToLowerInvariant(), this.name.ToLowerInvariant());
        }
    }

    public class VersionApiIncrement
    {
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.Username))]
        public string username { get; set; }

        [Required, StringLength(100)]
        public string name { get; set; }

        [Required]
        public VersionApiIncrementOption inc { get; set; }

        // By default, reset all the following version specifiers
        public bool reset { get; set; } = true;

        public TableOperation GetExistingOperation()
        {
            return TableOperation.Retrieve<VersionEntity>(this.username.ToLowerInvariant(), this.name.ToLowerInvariant());
        }
    }

    public enum VersionApiIncrementOption
    {
        Major,
        Minor,
        Patch
    }
}
