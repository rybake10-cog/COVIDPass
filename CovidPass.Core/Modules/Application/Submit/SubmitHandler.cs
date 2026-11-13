// © 2020 Partners HealthCare Technology & Innovation Department
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CovidPass.Common.Utils;
using CovidPass.Core.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using CovidPass.Common.Extensions;
using CovidPass.Db;
using CovidPass.Core.External.Email;
using System;

namespace CovidPass.Core.Modules.Application.Submit
{
    public class SubmitHandler : CommandHandler<SubmitCommand, ApplicationResponse>
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SubmitHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDatabase _db;
        private readonly IEmailService _emailService;

        public SubmitHandler(IConfiguration config, ILogger<SubmitHandler> logger,
            IHttpContextAccessor httpContextAccessor, IDatabase database, IEmailService emailService)
        {
            _config = config;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _db = database;
            _emailService = emailService;
        }

        public override async Task<ApplicationResponse> Handle(SubmitCommand model)
        {
            var created = DateTimeHelper.Now;
            var login = _httpContextAccessor.HttpContext.User.GetLogin();
            var subject = model.NoSymptoms
                ? $"COVID19 Day Pass - CLEARED FOR WORK {created:MM/dd/yyyy}"
                : $"COVID19 Day Pass - NOT CLEARED FOR WORK {created:MM/dd/yyyy}";

            if (model.IsManual)
            {
                await _db.InsertApplication(model.LastName, model.FirstName, model.Login, model.EmployeeId, model.Email,
                    model.Site, model.Initials, model.NoSymptoms, model.Influenza, model.Fever, model.Cough, model.ShortnessOfBreath,
                    model.SoreThroat, model.DiarrheaNausea, model.RunnyNose, model.MuscleAche, model.NewLossOfSmell, model.Language, login, created);
                if (!string.IsNullOrWhiteSpace(model.Email))
                {
                    var body = GenerateEmail(model.NoSymptoms, model.FirstName, model.LastName, model.EmployeeId, model.Email, created);
                    await _emailService.Send(model.Email, subject, body);
                }
                return new ApplicationResponse
                {
                    NoSymptoms = model.NoSymptoms,
                    LoginOrEmployeeId = model.EmployeeId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Date = created.Date,
                    Created = created
                };
            }
            else
            {
                var firstName = _httpContextAccessor.HttpContext.User.GetFirstName();
                var lastName = _httpContextAccessor.HttpContext.User.GetLastName();
                var email = _httpContextAccessor.HttpContext.User.GetEmail();
                var employeeId = _httpContextAccessor.HttpContext.User.GetUserId();
                await _db.InsertApplication(lastName, firstName, login, employeeId, email, model.Site, model.Initials,
                    model.NoSymptoms, model.Influenza, model.Fever, model.Cough, model.ShortnessOfBreath,
                    model.SoreThroat, model.DiarrheaNausea, model.RunnyNose, model.MuscleAche, model.NewLossOfSmell, model.Language, login, created);
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var body = GenerateEmail(model.NoSymptoms, firstName, lastName, login, email, created);
                    await _emailService.Send(model.Email, subject, body);
                }
                return new ApplicationResponse
                {
                    NoSymptoms = model.NoSymptoms,
                    LoginOrEmployeeId = login,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Date = created.Date,
                    Created = created
                };
            }
        }

        private string GenerateEmail(bool noSymptoms, string firstName, string lastName, string loginOrEmployeeId, string email, DateTime created)
        {
            var color = noSymptoms ? "#339966" : "#d90939";
            var title = noSymptoms ? "CLEARED FOR WORK" : "NOT CLEARED FOR WORK";
            var subTitle = noSymptoms
                ? "You are cleared for work and may start your shift after getting a mask from your location’s distribution center."
                : "You are NOT cleared for work and may not enter your building. Please contact your manager and Occupational Health at <a href=\"tel:+1xxxxxxxxxx\">(xxx)-xxx-xxxx</a>. <br />";
            var day = created.ToString("ddd").ToUpper();
            var dateTime = created.ToString("MM/dd/yyyy HH:mm:ss");
            string[] backGround = { "Purple", "Blue", "Green", "Yellow", "Brown", "Pink", "Orange" };
            string[] textColor = { "White", "White", "White", "Black", "White", "Black", "Black" };
            string[] weekDays = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            string bgColor = backGround[Array.IndexOf(weekDays, created.ToString("dddd"))];
            string txtColor = textColor[Array.IndexOf(weekDays, created.ToString("dddd"))];
            return $@"
<div style=""text-align: center; font-size: 32px; line-height: 32px; padding: 1px; background: rgb(255, 255, 255);"">
    <div style=""background-color: {bgColor}; color: {txtColor}; font-size: 50px; font-weight: bold; line-height: 70px;"">COVID Pass</div>
    <div style=""color: {color}; font-size: 60px; line-height: 60px; margin: 20px 0px;"">{title}</div>
    <div style=""font-size: 18px; line-height: 18px;"">{subTitle}</div>
    <div style=""border-bottom: 2px solid rgb(204, 204, 204); font-size: 60px; line-height: 60px; margin-top: 5px;"">{day}</div>
    <div style=""border-bottom: 2px solid rgb(204, 204, 204); margin-top: 8px; padding-bottom: 10px;"">{dateTime}</div>
    <div style=""font-size: 80px; line-height: 80px;"">{firstName} {lastName}</div>
</div>";
        }
    }
}
