// © 2020 Partners HealthCare Technology & Innovation Department
using System.Collections.Generic;

namespace CovidPass.Core.Modules.Auth.GetConfig
{
    public class Config
    {
        public bool UseLookup { get; set; }
        public List<string> Sites { get; set; }
    }
}
