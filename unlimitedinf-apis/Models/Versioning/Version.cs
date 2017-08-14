using Microsoft.WindowsAzure.Storage.Table;
using Unlimitedinf.Apis.Contracts.Versioning;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Models.Versioning
{
    public class VersionEntity : TableEntity
    {
        [IgnoreProperty]
        public string Username
        {
            get
            {
                return this.PartitionKey;
            }
            set
            {
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

        public VersionEntity() { }

        public VersionEntity(Version version)
        {
            this.Username = version.username;
            this.Name = version.name;
            this.Version = version.version;
        }

        public static implicit operator Version(VersionEntity entity)
        {
            if (entity == null)
                return null;

            return new Version
            {
                username = entity.Username,
                name = entity.Name,
                version = entity.Version
            };
        }
    }

    public static class VersionExtensions
    {
        public static TableOperation GetExistingOperation(this Version version)
        {
            return TableOperation.Retrieve<VersionEntity>(version.username.ToLowerInvariant(), version.name.ToLowerInvariant());
        }

        public static TableOperation GetExistingOperation(this VersionIncrement versionInc)
        {
            return TableOperation.Retrieve<VersionEntity>(versionInc.username.ToLowerInvariant(), versionInc.name.ToLowerInvariant());
        }
    }
}
