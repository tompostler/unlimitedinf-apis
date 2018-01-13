using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace BatchDelete
{
    internal class Entity : TableEntity
    {
        public static Guid DefaultPartitionKey = Guid.Empty;

        public Entity()
        {
            if (DefaultPartitionKey != Guid.Empty)
                this.PartitionKey = DefaultPartitionKey.ToString();
        }

        public Entity(int rowKey)
            : this()
        {
            this.RowKey = rowKey.ToString();
        }

        public Guid Thing1 { get; set; } = Guid.NewGuid();
        public Guid Thing2 { get; set; } = Guid.NewGuid();
        public Guid Thing3 { get; set; } = Guid.NewGuid();
        public Guid Thing4 { get; set; } = Guid.NewGuid();
    }
}
