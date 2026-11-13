// © 2020 Partners HealthCare Technology & Innovation Department
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CovidPass.Core.Modules.Auth.GetAuthSettings;
using CovidPass.Core.Modules.Auth.GetConfig;
using CovidPass.Core.Modules.Auth.GetCurrentUser;
using CovidPass.Core.Modules.Auth.GetWebToken;

namespace CovidPass.Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(nameof(GetToken))]
        public Task<AuthInfoModel> GetToken([FromBody]GetWebTokenQuery model) => _mediator.Send(model);

        [AllowAnonymous]
        [HttpGet(nameof(GetAuthSettings))]
        public Task<AuthSettings> GetAuthSettings([FromQuery]GetAuthSettingsQuery model) => _mediator.Send(model);

        [Authorize]
        [HttpGet(nameof(GetCurrentUser))]
        public Task<AuthInfoModel> GetCurrentUser([FromQuery]GetCurrentUserQuery model) => _mediator.Send(model);

        [Authorize]
        [HttpGet(nameof(GetConfig))]
        public Task<Config> GetConfig([FromQuery]GetConfigQuery model) => _mediator.Send(model);
    }
}
