// © 2020 Partners HealthCare Technology & Innovation Department
using CovidPass.Core.Infrastructure.Models;

namespace CovidPass.Core.Modules.Application.Submit
{
    public class SubmitCommand : Command<ApplicationResponse>
    {
        public string EmployeeId { get; set; }
        public string Login { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public bool NoSymptoms { get; set; }
        public bool Influenza { get; set; }
        public bool Fever { get; set; }
        public bool Cough { get; set; }
        public bool ShortnessOfBreath { get; set; }
        public bool SoreThroat { get; set; }
        public bool DiarrheaNausea { get; set; }
        public bool NewLossOfSmell { get; set; }
        public bool MuscleAche { get; set; }
        public bool RunnyNose { get; set; }
        public bool IsManual { get; set; }
        public string Site { get; set; }
        public string Initials { get; set; }
        public string Language { get; set; }
    }
}
