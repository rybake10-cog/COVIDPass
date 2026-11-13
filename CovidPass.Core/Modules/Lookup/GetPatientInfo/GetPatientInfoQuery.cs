// © 2020 Partners HealthCare Technology & Innovation Department
using CovidPass.Core.Infrastructure.Models;

namespace CovidPass.Core.Modules.Lookup.GetPatientInfo
{
    public class GetPatientInfoQuery : Query<PatientInfo>
    {
        public string Login { get; set; }
        public string EmployeeId { get; set; }
    }
}
