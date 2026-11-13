using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CovidPass.Common.Extensions;
using CovidPass.Core.External.Auth;
using CovidPass.Core.Models.Config;
using Xunit;

namespace CovidPass.Tests
{
    public class EntraIdClaimsTransformationTests
    {
        private static ClaimsPrincipal EntraPrincipal() => new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("preferred_username", "jdoe@example.org"),
            new Claim("given_name", "Jane"),
            new Claim("family_name", "Doe"),
            new Claim("email", "jane.doe@example.org"),
            new Claim("employeeid", "1234567"),
            new Claim("department", "Oncology"),
        }, "Bearer"));

        [Fact]
        public async Task MapsEntraClaimsToClaimsUsedByHandlers()
        {
            var principal = await new EntraIdClaimsTransformation(new EntraIdConfigModel()).TransformAsync(EntraPrincipal());

            Assert.Equal("jdoe@example.org", principal.GetLogin());
            Assert.Equal("1234567", principal.GetUserId());
            Assert.Equal("Jane", principal.GetFirstName());
            Assert.Equal("Doe", principal.GetLastName());
            Assert.Equal("jane.doe@example.org", principal.GetEmail());
            Assert.Equal("Oncology", principal.Get("DepartmentName"));
        }

        [Fact]
        public async Task IsIdempotent()
        {
            var transformation = new EntraIdClaimsTransformation(new EntraIdConfigModel());
            var principal = await transformation.TransformAsync(EntraPrincipal());
            principal = await transformation.TransformAsync(principal);

            Assert.Single(principal.FindAll(ClaimTypes.Name));
        }

        [Fact]
        public async Task UsesConfiguredClaimNames()
        {
            var identity = new ClaimsIdentity(new[] { new Claim("upn", "jdoe@example.org"), new Claim("extn.empid", "42") }, "Bearer");
            var config = new EntraIdConfigModel { LoginClaim = "upn", EmployeeIdClaim = "extn.empid" };

            var principal = await new EntraIdClaimsTransformation(config).TransformAsync(new ClaimsPrincipal(identity));

            Assert.Equal("jdoe@example.org", principal.GetLogin());
            Assert.Equal("42", principal.GetUserId());
        }

        [Fact]
        public async Task IgnoresAnonymousPrincipal()
        {
            var principal = await new EntraIdClaimsTransformation(new EntraIdConfigModel()).TransformAsync(new ClaimsPrincipal(new ClaimsIdentity()));

            Assert.Empty(principal.Claims);
        }
    }
}
