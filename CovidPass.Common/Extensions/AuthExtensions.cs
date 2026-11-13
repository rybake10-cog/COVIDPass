// © 2020 Partners HealthCare Technology & Innovation Department
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CovidPass.Common.Extensions
{
    public static class AuthExtensions
    {
        public static string GetLogin(this ClaimsPrincipal self)
        {
            return self.FindFirst(x => x.Type == ClaimTypes.Name)?.Value;
        }

        public static string GetFullName(this ClaimsPrincipal self)
        {
            var lastName = self?.FindFirst(x => x.Type == ClaimTypes.Surname)?.Value;
            var firstName = self?.FindFirst(x => x.Type == ClaimTypes.GivenName)?.Value;

            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
                return GetLogin(self);

            return string.IsNullOrWhiteSpace(lastName)
                ? firstName
                : firstName + " " + lastName;
        }

        public static string GetFirstName(this ClaimsPrincipal self)
        {
            return self?.FindFirst(x => x.Type == ClaimTypes.GivenName)?.Value;
        }

        public static string GetLastName(this ClaimsPrincipal self)
        {
            return self?.FindFirst(x => x.Type == ClaimTypes.Surname)?.Value;
        }

        public static string GetEmail(this ClaimsPrincipal self)
        {
            return self?.FindFirst(x => x.Type == ClaimTypes.Email)?.Value;
        }

        public static string GetUserId(this ClaimsPrincipal self)
        {
            return self?.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetUserData(this ClaimsPrincipal self)
        {
            return self.FindFirst(x => x.Type == ClaimTypes.UserData)?.Value;
        }

        public static string Get(this ClaimsPrincipal self, string claimTypeName)
        {
            return self.FindFirst(x => x.Type == claimTypeName)?.Value;
        }

        public static List<string> GetRoles(this ClaimsPrincipal self)
        {
            return self.FindAll(x => x.Type == ClaimTypes.Role)?.Select(x => x.Value).ToList() ?? new List<string>();
        }
    }
}
