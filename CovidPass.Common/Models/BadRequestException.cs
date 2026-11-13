// © 2020 Partners HealthCare Technology & Innovation Department
using System;

namespace CovidPass.Common.Models
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
