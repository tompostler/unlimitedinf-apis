using Unlimitedinf.Apis.Contracts.Axioms;

namespace Unlimitedinf.Apis.Server.Models.Axioms
{
    public class HttpAxiomEntity : AxiomBaseEntity
    {
        public const string PartitionKeyValue = "http";

        public HttpAxiomEntity()
        {
            this.Type = PartitionKeyValue;
        }

        public HttpAxiomEntity(AxiomBase axiom) : this()
        {
            this.Id = axiom.id;
            this.Timestamp = axiom.mod;
            this.Summary = axiom.sum;
            this.Description = axiom.desc;
            this.Source = axiom.src;
        }

        public static implicit operator AxiomBase(HttpAxiomEntity entity)
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