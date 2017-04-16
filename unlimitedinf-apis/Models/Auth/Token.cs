using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Globalization;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Models.Auth
{
    public class TokenEntity : TableEntity
    {
        public string Username { get; set; }

        public string Name { get; set; }

        [IgnoreProperty]
        public string Token
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

        public DateTime Expiration { get; set; }

        public TokenEntity() { }

        public TokenEntity(Token token)
        {
            this.PartitionKey = TokenExtensions.PartitionKey;
            this.Username = token.username;
            this.Name = token.name;
            this.Token = token.token;
            this.Expiration = token.expiration;
        }

        public TokenEntity(TokenCreate token)
        {
            this.PartitionKey = TokenExtensions.PartitionKey;
            this.Username = token.username;
            this.Name = token.name;
            switch (token.expire)
            {
                case TokenExpiration.Minute:
                    this.Expiration = DateTime.UtcNow.AddMinutes(1);
                    break;

                case TokenExpiration.Hour:
                    this.Expiration = DateTime.UtcNow.AddHours(1);
                    break;

                case TokenExpiration.Day:
                    this.Expiration = DateTime.UtcNow.AddDays(1);
                    break;

                case TokenExpiration.Week:
                    this.Expiration = DateTime.UtcNow.AddDays(7);
                    break;

                case TokenExpiration.Month:
                    this.Expiration = DateTime.UtcNow.AddMonths(1);
                    break;

                case TokenExpiration.Quarter:
                    this.Expiration = DateTime.UtcNow.AddMonths(3);
                    break;

                case TokenExpiration.Year:
                    this.Expiration = DateTime.UtcNow.AddYears(1);
                    break;

                case TokenExpiration.Never:
                    this.Expiration = DateTime.MaxValue;
                    break;
            }

            // <datetime string> <username> <hex fill to 48 chars>
            // Since usernames cannot contain whitespace, this will work
            this.Token = string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1} {2}",
                this.Expiration.ToString(TokenExtensions.DateTimeFmt),
                this.Username,
                GenerateRandom.HexToken(40)
                ).Chop(48).ToBase64String();
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
        public const string PartitionKey = "tokens";
        public const string DateTimeFmt = "yyMMddHHmmss";

        public static TableQuery<TokenEntity> GetExistingOperation(this TokenCreate token)
        {
            // WHERE Username='token.username' AND Name='token.name'
            return new TableQuery<TokenEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition(
                        nameof(TokenEntity.Username),
                        QueryComparisons.Equal,
                        token.username),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(
                        nameof(TokenEntity.Name),
                        QueryComparisons.Equal,
                        token.name)));
        }

        public static TableOperation GetExistingOperation(this Token token)
        {
            return GetExistingOperation(token.token);
        }

        public static TableOperation GetExistingOperation(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            return TableOperation.Retrieve<TokenEntity>(
                PartitionKey,
                token);
        }

        public static bool IsTokenExpired(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException();

            token = token.FromBase64String();
            if (token.Length < DateTimeFmt.Length)
                return true;

            DateTime tdt;
            if (!DateTime.TryParseExact(token.Chop(DateTimeFmt.Length), DateTimeFmt, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out tdt))
                return true;

            return tdt < DateTime.UtcNow;
        }
    }
}