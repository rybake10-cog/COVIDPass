// © 2020 Partners HealthCare Technology & Innovation Department
using System;

namespace CovidPass.Core.Modules.Auth.GetWebToken
{
    public class AuthInfoModel
    {
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmployeeNumber { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentNumber { get; set; }
        public bool IsAuth { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
