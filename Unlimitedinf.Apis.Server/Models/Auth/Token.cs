﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Globalization;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Server.Models.Auth
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

        public DateTimeOffset Expiration { get; set; }

        public TokenEntity() { }

        public TokenEntity(Token token)
        {
            this.PartitionKey = TokenExtensions.PartitionKey + token.username.ToLowerInvariant();
            this.Username = token.username;
            this.Name = token.name;
            this.Token = token.token;
            this.Expiration = token.expiration;
        }

        public TokenEntity(TokenCreate token)
        {
            this.PartitionKey = TokenExtensions.PartitionKey + token.username.ToLowerInvariant();
            this.Username = token.username;
            this.Name = token.name;
            switch (token.expire)
            {
                case TokenExpiration.minute:
                    this.Expiration = DateTimeOffset.UtcNow.AddMinutes(1);
                    break;

                case TokenExpiration.hour:
                    this.Expiration = DateTimeOffset.UtcNow.AddHours(1);
                    break;

                case TokenExpiration.day:
                    this.Expiration = DateTimeOffset.UtcNow.AddDays(1);
                    break;

                case TokenExpiration.week:
                    this.Expiration = DateTimeOffset.UtcNow.AddDays(7);
                    break;

                case TokenExpiration.month:
                    this.Expiration = DateTimeOffset.UtcNow.AddMonths(1);
                    break;

                case TokenExpiration.quarter:
                    this.Expiration = DateTimeOffset.UtcNow.AddMonths(3);
                    break;

                case TokenExpiration.year:
                    this.Expiration = DateTimeOffset.UtcNow.AddYears(1);
                    break;

                case TokenExpiration.never:
                    this.Expiration = DateTimeOffset.MaxValue;
                    break;
            }

            // <datetime string> <username> <hex fill to 96 chars>
            // Since usernames cannot contain whitespace, this will work
            this.Token = string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1} {2}",
                this.Expiration.ToString(Contracts.Auth.Token.DateTimeFmt),
                this.Username,
                GenerateRandom.HexToken(96)
                ).Chop(96).ToBase64String();
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
        public const string PartitionKey = "tokens_";
        public const string PartionKeyLessThan = "tokens`";

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
            return GetExistingOperation(token.username, token.token);
        }

        public static TableOperation GetExistingOperation(string username, string token)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            return TableOperation.Retrieve<TokenEntity>(
                PartitionKey + username.ToLowerInvariant(),
                token);
        }

        public static TableOperation GetExistingOperation(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var ctoken = token.FromBase64String();
            int beg = ctoken.IndexOf(' ') + 1;
            if (beg < 0 || beg >= ctoken.Length)
                throw new ArgumentException("Token is not well-formed.", nameof(token));
            int end = ctoken.IndexOf(' ', beg);
            if (end < 0)
                throw new ArgumentException("Token is not well-formed.", nameof(token));

            var username = ctoken.Substring(beg, end - beg);

            return GetExistingOperation(username, token);
        }
    }
}