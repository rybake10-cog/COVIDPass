using System;
using System.Threading.Tasks;
using CovidPass.Common.Extensions;
using CovidPass.Core.Infrastructure.Models;
using CovidPass.Core.Modules.Auth.GetWebToken;
using Microsoft.AspNetCore.Http;

namespace CovidPass.Core.Modules.Auth.GetCurrentUser
{
    public class GetCurrentUserHandler : QueryHandler<GetCurrentUserQuery, AuthInfoModel>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCurrentUserHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<AuthInfoModel> Handle(GetCurrentUserQuery model)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var exp = user.Get("exp");
            return Task.FromResult(new AuthInfoModel
            {
                Login = user.GetLogin(),
                FirstName = user.GetFirstName(),
                LastName = user.GetLastName(),
                Email = user.GetEmail(),
                EmployeeNumber = user.GetUserId(),
                DepartmentName = user.Get("DepartmentName"),
                DepartmentNumber = user.Get("DepartmentNumber"),
                IsAuth = true,
                Expires = long.TryParse(exp, out var seconds)
                    ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime
                    : DateTime.UtcNow
            });
        }
    }
}
