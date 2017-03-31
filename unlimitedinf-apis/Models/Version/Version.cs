using Microsoft.WindowsAzure.Storage.Table;
using Unlimitedinf.Apis.Contracts.Version;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Models.Version
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

    public class VersionApi : Contracts.Version.Version
    {
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

    public class VersionApiIncrement : VersionIncrement
    {
        public TableOperation GetExistingOperation()
        {
            return TableOperation.Retrieve<VersionEntity>(this.username.ToLowerInvariant(), this.name.ToLowerInvariant());
        }
    }
}
