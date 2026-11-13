// © 2020 Partners HealthCare Technology & Innovation Department
using System;

namespace CovidPass.Core.Modules.Application.Submit
{
    public class ApplicationResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string LoginOrEmployeeId { get; set; }
        public bool NoSymptoms { get; set; }
        public DateTime Date { get; set; }
        public DateTime Created { get; set; }
    }
}
