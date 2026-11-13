using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CovidPass.Core.Models.Config;
using Microsoft.AspNetCore.Authentication;

namespace CovidPass.Core.External.Auth
{
    /// <summary>
    /// Maps Entra ID access-token claims onto the claim types the app already reads
    /// (see AuthExtensions), so handlers work unchanged in either auth mode.
    /// </summary>
    public class EntraIdClaimsTransformation : IClaimsTransformation
    {
        internal const string MappedMarker = "covidpass:entra_mapped";
        private readonly EntraIdConfigModel _config;

        public EntraIdClaimsTransformation(EntraIdConfigModel config)
        {
            _config = config;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is ClaimsIdentity identity && identity.IsAuthenticated && !identity.HasClaim(x => x.Type == MappedMarker))
            {
                identity.AddClaims(MapClaims(principal, _config));
                identity.AddClaim(new Claim(MappedMarker, "true"));
            }
            return Task.FromResult(principal);
        }

        internal static IEnumerable<Claim> MapClaims(ClaimsPrincipal principal, EntraIdConfigModel config)
        {
            string Find(params string[] types) =>
                types.Select(t => principal.FindFirst(t)?.Value).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)) ?? string.Empty;

            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, Find(config.LoginClaim, "preferred_username", "upn")),
                new Claim(ClaimTypes.NameIdentifier, Find(config.EmployeeIdClaim)),
                new Claim(ClaimTypes.GivenName, Find("given_name")),
                new Claim(ClaimTypes.Surname, Find("family_name")),
                new Claim(ClaimTypes.Email, Find("email")),
                new Claim("DepartmentName", Find(config.DepartmentClaim)),
                new Claim("DepartmentNumber", string.Empty),
            };
        }
    }
}
