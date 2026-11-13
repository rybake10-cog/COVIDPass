using Microsoft.Data.SqlClient;
using Xunit;

namespace CovidPass.Tests
{
    public class AzureSqlTests
    {
        [Fact]
        public void ManagedIdentityAuthenticationProviderIsAvailable()
        {
            Assert.NotNull(SqlAuthenticationProvider.GetProvider(SqlAuthenticationMethod.ActiveDirectoryDefault));
            Assert.NotNull(SqlAuthenticationProvider.GetProvider(SqlAuthenticationMethod.ActiveDirectoryManagedIdentity));
        }
    }
}
