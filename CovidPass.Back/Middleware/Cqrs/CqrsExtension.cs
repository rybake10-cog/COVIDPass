// © 2020 Partners HealthCare Technology & Innovation Department
using Microsoft.AspNetCore.Builder;

namespace CovidPass.Back.Middleware.Cqrs
{
    public static class CqrsExtension
    {
        public static IApplicationBuilder UseCqrs(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CqrsMiddleware>();
        }
    }
}
