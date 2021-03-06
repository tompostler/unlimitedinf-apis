﻿using System;
using Unlimitedinf.Apis.Contracts;
using Unlimitedinf.Apis.Contracts.Auth;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public static class TAssert
    {
        public static void Equal(Account expected, Account actual)
        {
            Assert.Equal(expected.username, actual.username);
            Assert.Equal(expected.email, actual.email);
        }

        public static void Equal(TokenCreate expected, Token actual)
        {
            Assert.Equal(expected.username, actual.username);
            Assert.Equal(expected.name, actual.name);
            switch (expected.expire)
            {
                case TokenExpiration.minute:
                    Assert.InRange(
                        actual.expiration,
                        DateTimeOffset.UtcNow.AddMinutes(1).AddSeconds(-10),
                        DateTimeOffset.UtcNow.AddMinutes(1).AddSeconds(10));
                    break;
                case TokenExpiration.hour:
                    Assert.InRange(
                        actual.expiration,
                        DateTimeOffset.UtcNow.AddHours(1).AddSeconds(-10),
                        DateTimeOffset.UtcNow.AddHours(1).AddSeconds(10));
                    break;
                case TokenExpiration.day:
                    Assert.InRange(
                        actual.expiration,
                        DateTimeOffset.UtcNow.AddDays(1).AddSeconds(-10),
                        DateTimeOffset.UtcNow.AddDays(1).AddSeconds(10));
                    break;
                case TokenExpiration.week:
                    Assert.InRange(
                        actual.expiration,
                        DateTimeOffset.UtcNow.AddDays(7).AddSeconds(-10),
                        DateTimeOffset.UtcNow.AddDays(7).AddSeconds(10));
                    break;
                case TokenExpiration.month:
                    Assert.InRange(
                        actual.expiration,
                        DateTimeOffset.UtcNow.AddMonths(1).AddSeconds(-10),
                        DateTimeOffset.UtcNow.AddMonths(1).AddSeconds(10));
                    break;
                case TokenExpiration.quarter:
                    Assert.InRange(
                        actual.expiration,
                        DateTimeOffset.UtcNow.AddMonths(3).AddSeconds(-10),
                        DateTimeOffset.UtcNow.AddMonths(3).AddSeconds(10));
                    break;
                case TokenExpiration.year:
                    Assert.InRange(
                        actual.expiration,
                        DateTimeOffset.UtcNow.AddYears(1).AddSeconds(-10),
                        DateTimeOffset.UtcNow.AddYears(1).AddSeconds(10));
                    break;
                case TokenExpiration.never:
                    Assert.Equal(DateTimeOffset.MaxValue, actual.expiration);
                    break;
            }
        }

        public static void Equal(Token expected, Token actual)
        {
            Assert.Equal(expected.username, actual.username);
            Assert.Equal(expected.name, actual.name);
            Assert.Equal(expected.token, actual.token);
        }

        public static void Equal(Contracts.Versioning.Version expected, Contracts.Versioning.Version actual)
        {
            Assert.Equal(expected.username, actual.username, ignoreCase: true);
            Assert.Equal(expected.name, actual.name);
            Assert.Equal(expected.version, actual.version);
        }

        public static void Equal(Contracts.Versioning.Count expected, Contracts.Versioning.Count actual)
        {
            Assert.Equal(expected.username, actual.username, ignoreCase: true);
            Assert.Equal(expected.name, actual.name);
            Assert.Equal(expected.count, actual.count);
        }

        public static void Equal(Message expected, Message actual)
        {
            Assert.Equal(expected.from, actual.from, ignoreCase: true);
            Assert.Equal(expected.to, actual.to, ignoreCase: true);
            Assert.Equal(expected.subject, actual.subject, ignoreCase: true);
            Assert.Equal(expected.rept, actual.rept);
            Assert.Equal(expected.message, actual.message);
            Assert.Equal(expected.part, actual.part);
            Assert.True(actual.id.HasValue);
        }

        public static void Equal(Repo expected, Repo actual)
        {
            Assert.Equal(expected.username, actual.username, ignoreCase: true);
            Assert.Equal(expected.name, actual.name);
            Assert.Equal(expected.repo.AbsoluteUri, actual.repo.AbsoluteUri);
            Assert.Equal(expected.gitusername, actual.gitusername);
            Assert.Equal(expected.gituseremail, actual.gituseremail);
        }

        public static void Equal(Catan expected, Catan actual)
        {
            Assert.Equal(expected.username, actual.username, ignoreCase: true);
            Assert.Equal(expected.name, actual.name);

            Assert.Equal(expected.rolls.Count, actual.rolls.Count);
            for (int i = 0; i < expected.rolls.Count; i++)
            {
                Assert.Equal(expected.rolls[i].r, actual.rolls[i].r);
                Assert.Equal(expected.rolls[i].y, actual.rolls[i].y);
                Assert.InRange(actual.rolls[i].d, expected.rolls[i].d.AddMilliseconds(-1), expected.rolls[i].d.AddMilliseconds(1));
            }
        }
    }
}
