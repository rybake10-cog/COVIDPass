// © 2020 Partners HealthCare Technology & Innovation Department
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CovidPass.Core.External.Auth;
using CovidPass.Core.Infrastructure.Models;
using System.Collections.Generic;

namespace CovidPass.Core.Modules.Auth.GetConfig
{
    public class GetConfigHandler : QueryHandler<GetConfigQuery, Config>
    {
        private readonly IConfiguration _config;
        private readonly IDirectoryService _directoryService;

        public GetConfigHandler(IConfiguration config, IDirectoryService directoryService)
        {
            _config = config;
            _directoryService = directoryService;
        }

        public override Task<Config> Handle(GetConfigQuery model)
        {
            var sites = _config.GetSection("Sites").Get<List<string>>();
            var useLookup = _config.GetValue<bool>("UseLookup");
            return Task.FromResult(new Config { Sites = sites, UseLookup = useLookup });
        }
    }
}
