// © 2020 Partners HealthCare Technology & Innovation Department
using CovidPass.Core.Infrastructure.Models;

namespace CovidPass.Core.Modules.Auth.GetWebToken
{
    public class GetWebTokenQuery : Query<AuthInfoModel>
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
