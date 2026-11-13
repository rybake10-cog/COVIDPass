// © 2020 Partners HealthCare Technology & Innovation Department
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CovidPass.Core.External.Auth;
using CovidPass.Core.Infrastructure.Models;

namespace CovidPass.Core.Modules.Lookup.GetPatientInfo
{
    public class GetPatientInfoHandler : QueryHandler<GetPatientInfoQuery, PatientInfo>
    {
        private readonly IUserLookupService _userLookupService;

        public GetPatientInfoHandler(IUserLookupService userLookupService)
        {
            _userLookupService = userLookupService;
        }

        public override async Task<PatientInfo> Handle(GetPatientInfoQuery model)
        {
            if (string.IsNullOrWhiteSpace(model.EmployeeId) && string.IsNullOrWhiteSpace(model.Login))
            {
                BadRequest("You should specify parameters");
                return null;
            }
            var employeeId = string.IsNullOrWhiteSpace(model.EmployeeId)
                ? model.EmployeeId
                : Regex.Replace(model.EmployeeId, @"^.*?(\d{1,9}).*?$", "$1");
            var info = await _userLookupService.FindUser(model.Login, employeeId);
            return new PatientInfo
            {
                EmployeeNumber = info?.EmployeeNumber,
                LastName = info?.LastName,
                FirstName = info?.FirstName,
                DepartmentName = info?.DepartmentName,
                DepartmentNumber = info?.DepartmentNumber,
                Username = info?.Username,
                Email = info?.Email
            };
        }
    }
}
