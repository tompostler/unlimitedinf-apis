using Microsoft.WindowsAzure.Storage.Table;
using System;
using Unlimitedinf.Apis.Contracts.Frequencies;

namespace Unlimitedinf.Apis.Server.Models.Frequencies
{
    public class SkittlesEntity : TableEntity
    {
        public const string PartitionKeyPrefix = "skittles_";

        [IgnoreProperty]
        public string Username
        {
            get
            {
                return this.PartitionKey;
            }
            set
            {
                this.PartitionKey = PartitionKeyPrefix + value.ToLowerInvariant();
            }
        }

        [IgnoreProperty]
        public string Id
        {
            get
            {
                return this.RowKey;
            }
        }

        [IgnoreProperty]
        public Skittles.SkittleType Type { get; set; }
        public string TSType
        {
            get
            {
                return this.Type.ToString();
            }
            set
            {
                this.Type = (Skittles.SkittleType)Enum.Parse(typeof(Skittles.SkittleType), value);
            }
        }

        [IgnoreProperty]
        public Skittles.SkittleSize Size { get; set; }
        public string TSSize
        {
            get
            {
                return this.Size.ToString();
            }
            set
            {
                this.Size = (Skittles.SkittleSize)Enum.Parse(typeof(Skittles.SkittleSize), value);
            }
        }

        [IgnoreProperty]
        public Skittles.SkittleColors Colors
        {
            get
            {
                switch (this.Type)
                {
                    case Skittles.SkittleType.classic:
                        return new Skittles.SkittleColorClassic
                        {
                            total = this.TSTotal,
                            purple = this.TSColor1,
                            red = this.TSColor2,
                            yellow = this.TSColor3,
                            orange = this.TSColor4,
                            green = this.TSColor5
                        };

                    default:
                        throw new NotImplementedException();
                }
            }
            set
            {
                switch (this.Type)
                {
                    case Skittles.SkittleType.classic:
                        var val = (Skittles.SkittleColorClassic)value;
                        this.TSTotal = val.total;
                        this.TSColor1 = val.purple;
                        this.TSColor2 = val.red;
                        this.TSColor3 = val.yellow;
                        this.TSColor4 = val.orange;
                        this.TSColor5 = val.green;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public int TSTotal { get; set; }
        public int TSColor1 { get; set; }
        public int TSColor2 { get; set; }
        public int TSColor3 { get; set; }
        public int TSColor4 { get; set; }
        public int TSColor5 { get; set; }

        public SkittlesEntity() { }

        public SkittlesEntity(Skittles skittles)
        {
            this.Username = skittles.username;
            //this.Id = skittles.id;
            this.Type = skittles.type;
            this.Size = skittles.size;
            this.Colors = skittles.colors;
        }

        public static implicit operator Skittles(SkittlesEntity entity)
        {
            if (entity == null)
                return null;

            return new Skittles
            {
                username = entity.Username,
                id = entity.Id,
                type = entity.Type,
                size = entity.Size,
                colors = entity.Colors
            };
        }
    }

    public static class SkittlesExtensions
    {
        public static TableOperation GetExistingOperation(this Skittles skittles)
        {
            return TableOperation.Retrieve<SkittlesEntity>(SkittlesEntity.PartitionKeyPrefix + skittles.username.ToLowerInvariant(), skittles.id);
        }
    }
}
