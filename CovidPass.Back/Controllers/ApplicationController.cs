// © 2020 Partners HealthCare Technology & Innovation Department
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CovidPass.Core.Modules.Application.Submit;

namespace CovidPass.Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost(nameof(Submit))]
        public Task<ApplicationResponse> Submit([FromBody]SubmitCommand model) => _mediator.Send(model);
    }
}
