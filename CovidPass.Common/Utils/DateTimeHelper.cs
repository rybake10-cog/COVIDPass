// © 2020 Partners HealthCare Technology & Innovation Department
using System;

namespace CovidPass.Common.Utils
{
    public class DateTimeHelper
    {
        private static string _timeZoneId = "Eastern Standard Time";
        public static DateTime UtcNow => DateTime.UtcNow;
        public static DateTime Now => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, _timeZoneId);
    }
}
