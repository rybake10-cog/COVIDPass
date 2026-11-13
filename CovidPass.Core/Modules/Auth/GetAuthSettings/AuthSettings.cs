namespace CovidPass.Core.Modules.Auth.GetAuthSettings
{
    public class AuthSettings
    {
        public string Mode { get; set; }
        public string ClientId { get; set; }
        public string Authority { get; set; }
        public string[] Scopes { get; set; }
    }
}
