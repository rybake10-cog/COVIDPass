using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using CovidPass.Core.External.Auth;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CovidPass.Tests
{
    public class GraphUserLookupServiceTests
    {
        [Theory]
        [InlineData("jdoe", null, "onPremisesSamAccountName eq 'jdoe'")]
        [InlineData("jdoe@example.org", null, "userPrincipalName eq 'jdoe@example.org' or mail eq 'jdoe@example.org'")]
        [InlineData(null, "1234567", "employeeId eq '1234567'")]
        [InlineData("o'brien", "1", "onPremisesSamAccountName eq 'o''brien'")]
        [InlineData(" ", " ", null)]
        public void BuildsODataFilter(string username, string employeeNumber, string expected)
        {
            Assert.Equal(expected, GraphUserLookupService.BuildFilter(username, employeeNumber));
        }

        [Fact]
        public async Task QueriesGraphWithManagedIdentityTokenAndMapsUser()
        {
            var handler = new StubHandler(HttpStatusCode.OK,
                "{\"value\":[{\"userPrincipalName\":\"jdoe@example.org\",\"onPremisesSamAccountName\":\"jdoe\",\"givenName\":\"Jane\",\"surname\":\"Doe\",\"mail\":\"jane.doe@example.org\",\"department\":\"Oncology\",\"employeeId\":\"1234567\"}]}");
            var credential = new StubCredential();
            var service = new GraphUserLookupService(new HttpClient(handler), credential, NullLogger<GraphUserLookupService>.Instance);

            var user = await service.FindUser(null, "1234567");

            Assert.Equal(GraphUserLookupService.GraphScope, Assert.Single(credential.RequestedScopes));
            Assert.Equal("Bearer", handler.Request.Headers.Authorization.Scheme);
            Assert.Equal("test-token", handler.Request.Headers.Authorization.Parameter);
            Assert.Contains("eventual", handler.Request.Headers.GetValues("ConsistencyLevel"));
            Assert.StartsWith(GraphUserLookupService.GraphUsersUrl + "?$filter=employeeId%20eq%20%271234567%27", handler.Request.RequestUri.AbsoluteUri);
            Assert.Equal("jdoe", user.Username);
            Assert.Equal("1234567", user.EmployeeNumber);
            Assert.Equal("Jane", user.FirstName);
            Assert.Equal("Doe", user.LastName);
            Assert.Equal("jane.doe@example.org", user.Email);
            Assert.Equal("Oncology", user.DepartmentName);
        }

        [Fact]
        public async Task ReturnsNullWhenUserNotFound()
        {
            var service = new GraphUserLookupService(new HttpClient(new StubHandler(HttpStatusCode.OK, "{\"value\":[]}")), new StubCredential(), NullLogger<GraphUserLookupService>.Instance);

            Assert.Null(await service.FindUser("nobody", null));
        }

        [Fact]
        public async Task ReturnsNullWhenGraphDeniesAccess()
        {
            var service = new GraphUserLookupService(new HttpClient(new StubHandler(HttpStatusCode.Forbidden, "{}")), new StubCredential(), NullLogger<GraphUserLookupService>.Instance);

            Assert.Null(await service.FindUser("jdoe", null));
        }

        private class StubHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _status;
            private readonly string _body;
            public HttpRequestMessage Request { get; private set; }

            public StubHandler(HttpStatusCode status, string body)
            {
                _status = status;
                _body = body;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Request = request;
                return Task.FromResult(new HttpResponseMessage(_status) { Content = new StringContent(_body, Encoding.UTF8, "application/json") });
            }
        }

        private class StubCredential : TokenCredential
        {
            public string[] RequestedScopes { get; private set; }

            public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
                GetTokenAsync(requestContext, cancellationToken).GetAwaiter().GetResult();

            public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                RequestedScopes = requestContext.Scopes;
                return new ValueTask<AccessToken>(new AccessToken("test-token", DateTimeOffset.UtcNow.AddHours(1)));
            }
        }
    }
}
