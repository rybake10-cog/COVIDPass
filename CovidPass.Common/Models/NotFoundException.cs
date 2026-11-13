// © 2020 Partners HealthCare Technology & Innovation Department
using System;

namespace CovidPass.Common.Models
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
