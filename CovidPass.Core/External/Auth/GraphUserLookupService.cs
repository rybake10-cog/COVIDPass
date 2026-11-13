using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.Extensions.Logging;

namespace CovidPass.Core.External.Auth
{
    /// <summary>
    /// Looks up employees in Microsoft Entra ID through Microsoft Graph, replacing the LDAP lookup.
    /// Requires the app's managed identity to have the Graph User.Read.All application permission.
    /// </summary>
    public class GraphUserLookupService : IUserLookupService
    {
        internal const string GraphScope = "https://graph.microsoft.com/.default";
        internal const string GraphUsersUrl = "https://graph.microsoft.com/v1.0/users";
        private const string Select = "userPrincipalName,onPremisesSamAccountName,givenName,surname,mail,department,employeeId";

        private readonly HttpClient _httpClient;
        private readonly TokenCredential _credential;
        private readonly ILogger<GraphUserLookupService> _logger;

        public GraphUserLookupService(HttpClient httpClient, TokenCredential credential, ILogger<GraphUserLookupService> logger)
        {
            _httpClient = httpClient;
            _credential = credential;
            _logger = logger;
        }

        public async Task<DirectoryUserInfo> FindUser(string username, string employeeNumber, CancellationToken cancellationToken = default)
        {
            var filter = BuildFilter(username, employeeNumber);
            if (filter == null)
                return null;

            var url = $"{GraphUsersUrl}?$filter={Uri.EscapeDataString(filter)}&$select={Select}&$count=true&$top=2";
            try
            {
                var token = await _credential.GetTokenAsync(new TokenRequestContext(new[] { GraphScope }), cancellationToken);
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                // Required for filtering on onPremisesSamAccountName (advanced query).
                request.Headers.Add("ConsistencyLevel", "eventual");

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Graph user lookup failed with status {StatusCode}", (int)response.StatusCode);
                    return null;
                }

                var payload = await response.Content.ReadFromJsonAsync<GraphUserList>(cancellationToken: cancellationToken);
                var user = payload?.Value?.FirstOrDefault();
                return user == null ? null : new DirectoryUserInfo
                {
                    Username = user.OnPremisesSamAccountName ?? user.UserPrincipalName,
                    EmployeeNumber = user.EmployeeId,
                    FirstName = user.GivenName,
                    LastName = user.Surname,
                    Email = user.Mail,
                    DepartmentName = user.Department
                };
            }
            catch (Exception e) when (e is HttpRequestException || e is Azure.Identity.AuthenticationFailedException || e is Azure.RequestFailedException)
            {
                _logger.LogError(e, "Graph user lookup failed");
                return null;
            }
        }

        internal static string BuildFilter(string username, string employeeNumber)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                var value = EscapeODataString(username.Trim());
                return username.Contains('@')
                    ? $"userPrincipalName eq '{value}' or mail eq '{value}'"
                    : $"onPremisesSamAccountName eq '{value}'";
            }
            if (!string.IsNullOrWhiteSpace(employeeNumber))
                return $"employeeId eq '{EscapeODataString(employeeNumber.Trim())}'";
            return null;
        }

        private static string EscapeODataString(string value) => value.Replace("'", "''");

        private class GraphUserList
        {
            [JsonPropertyName("value")]
            public List<GraphUser> Value { get; set; }
        }

        private class GraphUser
        {
            [JsonPropertyName("userPrincipalName")] public string UserPrincipalName { get; set; }
            [JsonPropertyName("onPremisesSamAccountName")] public string OnPremisesSamAccountName { get; set; }
            [JsonPropertyName("givenName")] public string GivenName { get; set; }
            [JsonPropertyName("surname")] public string Surname { get; set; }
            [JsonPropertyName("mail")] public string Mail { get; set; }
            [JsonPropertyName("department")] public string Department { get; set; }
            [JsonPropertyName("employeeId")] public string EmployeeId { get; set; }
        }
    }
}
