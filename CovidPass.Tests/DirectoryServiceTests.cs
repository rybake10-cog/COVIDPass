using CovidPass.Core.External.Auth;
using Xunit;

namespace CovidPass.Tests
{
    public class DirectoryServiceTests
    {
        [Theory]
        [InlineData("jdoe", "jdoe")]
        [InlineData("*)(uid=*", "\\2a\\29\\28uid=\\2a")]
        [InlineData("a\\b", "a\\5cb")]
        public void EscapesLdapFilterValues(string value, string expected)
        {
            Assert.Equal(expected, DirectoryService.EscapeLdapFilter(value));
        }
    }
}
