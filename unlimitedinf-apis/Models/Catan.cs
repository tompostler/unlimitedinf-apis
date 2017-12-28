using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using Unlimitedinf.Apis.Contracts;

namespace Unlimitedinf.Apis.Models
{
    public class CatanEntity : TableEntity
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
        public IList<Catan.Roll> Rolls { get; set; } = new List<Catan.Roll>();

        /// <summary>
        /// Added to store the rolls in table storage since TS is not good at nested types...
        /// </summary>
        public string TSRolls
        {
            get
            {
                return JsonConvert.SerializeObject(Rolls);
            }
            set
            {
                Rolls = JsonConvert.DeserializeObject<List<Catan.Roll>>(value);
            }
        }

        public CatanEntity() { }

        public CatanEntity(Catan catan)
        {
            this.Username = catan.username;
            this.Name = catan.name;
            this.Rolls = catan.rolls;
        }

        public static implicit operator Catan(CatanEntity entity)
        {
            if (entity == null)
                return null;

            return new Catan
            {
                username = entity.Username,
                name = entity.Name,
                rolls = entity.Rolls
            };
        }
    }

    public static class CatanExtensions
    {
        public static TableOperation GetExistingOperation(this Catan catan)
        {
            return TableOperation.Retrieve<CatanEntity>(catan.username.ToLowerInvariant(), catan.name.ToLowerInvariant());
        }
    }
}
