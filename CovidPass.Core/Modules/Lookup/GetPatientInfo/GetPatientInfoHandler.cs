// © 2020 Partners HealthCare Technology & Innovation Department
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CovidPass.Core.External.Auth;
using CovidPass.Core.Infrastructure.Models;
using Microsoft.Extensions.Configuration;

namespace CovidPass.Core.Modules.Lookup.GetPatientInfo
{
    public class GetPatientInfoHandler : QueryHandler<GetPatientInfoQuery, PatientInfo>
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDirectoryService _directoryService;

        public GetPatientInfoHandler(IConfiguration config, IHttpClientFactory httpClientFactory, IDirectoryService directoryService)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _directoryService = directoryService;
        }

        public override Task<PatientInfo> Handle(GetPatientInfoQuery model)
        {
            if (string.IsNullOrWhiteSpace(model.EmployeeId) && string.IsNullOrWhiteSpace(model.Login))
            {
                BadRequest("You should specify parameters");
                return null;
            }

            var serviceUser = _config.GetValue<string>("ServiceUser2");
            var servicePassword = _config.GetValue<string>("ServicePassword2");
            var employeeId = string.IsNullOrWhiteSpace(model.EmployeeId)
                ? model.EmployeeId
                : Regex.Replace(model.EmployeeId, @"^.*?(\d{1,9}).*?$", "$1");
            var info = _directoryService.GetUserInfo(serviceUser, servicePassword, model.Login, employeeId);
            return Task.FromResult(new PatientInfo
            {
                EmployeeNumber = info?.EmployeeNumber,
                LastName = info?.LastName,
                FirstName = info?.FirstName,
                DepartmentName = info?.DepartmentName,
                DepartmentNumber = info?.DepartmentNumber,
                Username = info?.Username,
                Email = info?.Email
            });
        }
    }
}
