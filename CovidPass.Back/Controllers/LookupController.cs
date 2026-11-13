// © 2020 Partners HealthCare Technology & Innovation Department
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CovidPass.Core.Modules.Lookup.GetPatientInfo;

namespace CovidPass.Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LookupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost(nameof(GetPatientInfo))]
        public Task<PatientInfo> GetPatientInfo([FromQuery]GetPatientInfoQuery model) => _mediator.Send(model);
    }
}
