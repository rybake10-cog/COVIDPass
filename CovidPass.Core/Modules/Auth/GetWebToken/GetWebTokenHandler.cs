// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CovidPass.Common.Utils;
using CovidPass.Core.External.Auth;
using CovidPass.Core.Infrastructure.Models;
using CovidPass.Core.Models.Config;
using Microsoft.Extensions.Logging;

namespace CovidPass.Core.Modules.Auth.GetWebToken
{
    public class GetWebTokenHandler : QueryHandler<GetWebTokenQuery, AuthInfoModel>
    {
        private readonly IConfiguration _config;
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<GetWebTokenHandler> _logger;

        public GetWebTokenHandler(IConfiguration config, IDirectoryService directoryService, ILogger<GetWebTokenHandler> logger)
        {
            _config = config;
            _directoryService = directoryService;
            _logger = logger;
        }

        public override Task<AuthInfoModel> Handle(GetWebTokenQuery model)
        {
            var authConfig = _config.GetSection("Auth").Get<AuthConfigModel>();

            if (!(string.IsNullOrWhiteSpace(model.Login) || string.IsNullOrWhiteSpace(model.Password)))
            {
                var domain = "YOUR_DOMAIN_NAME";
                var serviceUser = _config.GetValue<string>("ServiceUser");
                var servicePassword = _config.GetValue<string>("ServicePassword");
                if (_directoryService.IsUserLocked(domain, serviceUser, servicePassword, model.Login))
                {
                    BadRequest("Account is locked. Too many attempts. Try again later.");
                    return null;
                }

                var validated = _directoryService.ValidateUser(domain, model.Login, model.Password);
                if (validated)
                {
                    var serviceUser2 = _config.GetValue<string>("ServiceUser2");
                    var servicePassword2 = _config.GetValue<string>("ServicePassword2");
                    var info = _directoryService.GetUserInfo(serviceUser2, servicePassword2, model.Login, null);
                    var expires = DateTimeHelper.UtcNow.AddMinutes(authConfig.Expires);
                    var userId = info?.Username ?? model.Login;

                    var identity = GetClaimsIdentity(userId, info);
                    var token = GetToken(identity, expires, authConfig);

                    return Task.FromResult(new AuthInfoModel
                    {
                        Login = userId,
                        Expires = expires,
                        IsAuth = true,
                        Token = token,
                        Email = info?.Email,
                        FirstName = info?.FirstName,
                        LastName = info?.LastName,
                        EmployeeNumber = info?.EmployeeNumber,
                        DepartmentName = info?.DepartmentName,
                        DepartmentNumber = info?.DepartmentNumber
                    });
                }
            }
            BadRequest("username/password incorrect");
            return null;
        }

        private string GetToken(ClaimsIdentity identity, DateTime expires, AuthConfigModel authConfig)
        {
            var handler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = handler.CreateJwtSecurityToken(subject: identity,
                signingCredentials: signingCredentials,
                audience: authConfig.Audience,
                issuer: authConfig.Issuer,
                expires: expires);

            return handler.WriteToken(token);
        }

        private ClaimsIdentity GetClaimsIdentity(string login, DirectoryUserInfo info)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login),
                new Claim(ClaimTypes.NameIdentifier, info?.EmployeeNumber ?? string.Empty),
                new Claim(ClaimTypes.GivenName, info?.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, info?.LastName ?? string.Empty),
                new Claim(ClaimTypes.Email, info?.Email ?? string.Empty),
                new Claim("DepartmentName", info?.DepartmentName ?? string.Empty),
                new Claim("DepartmentNumber", info?.DepartmentNumber ?? string.Empty),
            };
            return new ClaimsIdentity(claims);
        }
    }
}
