using System;
using System.Linq;

namespace CovidPass.Core.Models.Config
{
    public class EntraIdConfigModel
    {
        public string Instance { get; set; } = "https://login.microsoftonline.com/";
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string Audience { get; set; }
        public string[] Scopes { get; set; }
        public string LoginClaim { get; set; } = "preferred_username";
        public string EmployeeIdClaim { get; set; } = "employeeid";
        public string DepartmentClaim { get; set; } = "department";

        public string Authority => $"{Instance.TrimEnd('/')}/{TenantId}/v2.0";

        public string ApplicationIdUri => string.IsNullOrWhiteSpace(Audience) ? $"api://{ClientId}" : Audience;

        public string[] ValidAudiences => new[] { ClientId, ApplicationIdUri }.Distinct().ToArray();

        public string[] EffectiveScopes => Scopes is { Length: > 0 } ? Scopes : new[] { $"{ApplicationIdUri}/access_as_user" };

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(TenantId) || string.IsNullOrWhiteSpace(ClientId))
                throw new InvalidOperationException("Auth:EntraId:TenantId and Auth:EntraId:ClientId are required when Auth:Mode is EntraId.");
        }
    }
}
