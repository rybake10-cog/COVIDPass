// © 2020 Partners HealthCare Technology & Innovation Department
using System.Collections.Generic;

namespace CovidPass.Core.Models.Config
{
    public class AuthConfigModel
    {
        public string Key { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string PasswordSalt { get; set; }
        public int Expires { get; set; }
    }
}
