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
    }
}
