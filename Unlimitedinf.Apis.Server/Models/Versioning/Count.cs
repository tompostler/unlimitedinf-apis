using Microsoft.WindowsAzure.Storage.Table;
using Unlimitedinf.Apis.Contracts.Versioning;

namespace Unlimitedinf.Apis.Server.Models.Versioning
{
    public class CountEntity : TableEntity
    {
        public const string PartitionKeySuffix = "_c";

        [IgnoreProperty]
        public string Username
        {
            get
            {
                return this.PartitionKey.Substring(0, this.PartitionKey.Length - PartitionKeySuffix.Length);
            }
            set
            {
                this.PartitionKey = value.ToLowerInvariant() + PartitionKeySuffix;
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

        public long Count { get; set; }

        public CountEntity() { }

        public CountEntity(Count count)
        {
            this.Username = count.username;
            this.Name = count.name;
            this.Count = count.count;
        }

        public static implicit operator Count(CountEntity entity)
        {
            return new Count
            {
                username = entity.Username,
                name = entity.Name,
                count = entity.Count
            };
        }
    }

    public static class CountExtensions
    {
        public static TableOperation GetExistingOperation(this Count count)
        {
            return TableOperation.Retrieve<CountEntity>(count.username.ToLowerInvariant() + CountEntity.PartitionKeySuffix, count.name.ToLowerInvariant());
        }

        public static TableOperation GetExistingOperation(string username, string countName)
        {
            return TableOperation.Retrieve<VersionEntity>(username.ToLowerInvariant() + CountEntity.PartitionKeySuffix, countName.ToLowerInvariant());
        }
    }
}
