using Microsoft.WindowsAzure.Storage.Table;
using Unlimitedinf.Apis.Contracts.Axioms;

namespace Unlimitedinf.Apis.Models.Axioms
{
    public class AxiomBaseEntity : TableEntity
    {
        [IgnoreProperty]
        public string Type
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
        public string Id
        {
            get
            {
                return this.RowKey;
            }
            set
            {
                this.RowKey = value.ToLowerInvariant();
            }
        }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string Source { get; set; }

        public AxiomBaseEntity() { }

        public AxiomBaseEntity(AxiomBase axiom)
        {
            this.Type = axiom.type;
            this.Id = axiom.id;
            this.Timestamp = axiom.mod;
            this.Summary = axiom.sum;
            this.Description = axiom.desc;
            this.Source = axiom.src;
        }

        public static implicit operator AxiomBase(AxiomBaseEntity entity)
        {
            return new AxiomBase
            {
                type = entity.Type,
                id = entity.Id,
                mod = entity.Timestamp,
                sum = entity.Summary,
                desc = entity.Description,
                src = entity.Source
            };
        }
    }
}
