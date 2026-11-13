using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CovidPass.Back;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CovidPass.Tests
{
    public class ApiTests
    {
        private static HttpClient CreateClient(Dictionary<string, string> settings) =>
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder(b => b.ConfigureAppConfiguration((_, c) => c.AddInMemoryCollection(settings)))
                .CreateClient();

        private static Dictionary<string, string> EntraSettings() => new Dictionary<string, string>
        {
            ["Auth:Mode"] = "EntraId",
            ["Auth:EntraId:TenantId"] = "11111111-1111-1111-1111-111111111111",
            ["Auth:EntraId:ClientId"] = "22222222-2222-2222-2222-222222222222",
        };

        [Fact]
        public async Task HealthEndpointResponds()
        {
            var response = await CreateClient(EntraSettings()).GetAsync("/api/health");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AuthSettingsExposeEntraConfigurationForTheSpa()
        {
            var settings = await CreateClient(EntraSettings()).GetFromJsonAsync<JsonElement>("/api/auth/getAuthSettings");

            Assert.Equal("EntraId", settings.GetProperty("mode").GetString());
            Assert.Equal("22222222-2222-2222-2222-222222222222", settings.GetProperty("clientId").GetString());
            Assert.Equal("https://login.microsoftonline.com/11111111-1111-1111-1111-111111111111", settings.GetProperty("authority").GetString());
            Assert.Equal("api://22222222-2222-2222-2222-222222222222/access_as_user", settings.GetProperty("scopes")[0].GetString());
        }

        [Fact]
        public async Task AuthSettingsDefaultToDirectoryMode()
        {
            var settings = await CreateClient(new Dictionary<string, string> { ["Auth:Key"] = "legacy-test-signing-key-0123456789abcdef" })
                .GetFromJsonAsync<JsonElement>("/api/auth/getAuthSettings");

            Assert.Equal("Directory", settings.GetProperty("mode").GetString());
        }

        [Theory]
        [InlineData("/api/auth/getConfig")]
        [InlineData("/api/auth/getCurrentUser")]
        public async Task ProtectedEndpointsRequireToken(string url)
        {
            var response = await CreateClient(EntraSettings()).GetAsync(url);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PasswordSignInIsDisabledInEntraMode()
        {
            var response = await CreateClient(EntraSettings()).PostAsJsonAsync("/api/auth/getToken", new { login = "jdoe", password = "secret" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Entra ID", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task PathPrefixIsRewrittenForContainerHosting()
        {
            var settings = EntraSettings();
            settings["RewriteUrlSection"] = "^employee/";

            var response = await CreateClient(settings).GetAsync("/employee/api/health");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
