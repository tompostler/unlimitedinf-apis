using Microsoft.WindowsAzure.Storage.Table;
using System;
using Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis.Models.Auth
{
    public class TokenEntity : TableEntity
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

        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        public TokenEntity() { }

        public TokenEntity(Token token)
        {
            this.Username = token.username;
            this.Name = token.name;
            this.Token = token.token;
            this.Expiration = token.expiration;
        }

        public static implicit operator Token(TokenEntity entity)
        {
            return new Token
            {
                expiration = entity.Expiration,
                name = entity.Name,
                token = entity.Token,
                username = entity.Username
            };
        }
    }

    public static class TokenExtensions
    {
        public static TableOperation GetExistingOperation(this Token token)
        {
            return TableOperation.Retrieve<TokenEntity>(
                token.username.ToLowerInvariant(),
                token.name);
        }
    }
}