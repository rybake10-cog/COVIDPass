// © 2020 Partners HealthCare Technology & Innovation Department
namespace CovidPass.Core.Models.Config
{
    public static class AuthModes
    {
        public const string Directory = "Directory";
        public const string EntraId = "EntraId";
    }

    public class AuthConfigModel
    {
        public string Mode { get; set; } = AuthModes.Directory;
        public string Key { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string PasswordSalt { get; set; }
        public int Expires { get; set; }
        public EntraIdConfigModel EntraId { get; set; } = new EntraIdConfigModel();

        public bool IsEntraId => string.Equals(Mode, AuthModes.EntraId, System.StringComparison.OrdinalIgnoreCase);
    }
}
