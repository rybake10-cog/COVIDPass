// © 2020 Partners HealthCare Technology & Innovation Department
using Microsoft.AspNetCore.Builder;

namespace CovidPass.Back.Middleware
{
    public static class ExceptionConverterExtension
    {
        public static IApplicationBuilder UseExceptionConverter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionConverterMiddleware>();
        }
    }
}
