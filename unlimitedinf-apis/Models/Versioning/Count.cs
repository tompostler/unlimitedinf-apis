using Microsoft.WindowsAzure.Storage.Table;
using Unlimitedinf.Apis.Contracts.Versioning;

namespace Unlimitedinf.Apis.Models.Versioning
{
    public class CountEntity : TableEntity
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
            return TableOperation.Retrieve<CountEntity>(count.username.ToLowerInvariant(), count.name.ToLowerInvariant());
        }

        public static TableOperation GetExistingOperation(this CountChange countInc)
        {
            return TableOperation.Retrieve<CountEntity>(countInc.username.ToLowerInvariant(), countInc.name.ToLowerInvariant());
        }
    }
}
