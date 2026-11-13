using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CovidPass.Core.External.Auth
{
    public class LdapUserLookupService : IUserLookupService
    {
        private readonly IConfiguration _config;
        private readonly IDirectoryService _directoryService;

        public LdapUserLookupService(IConfiguration config, IDirectoryService directoryService)
        {
            _config = config;
            _directoryService = directoryService;
        }

        public Task<DirectoryUserInfo> FindUser(string username, string employeeNumber, CancellationToken cancellationToken = default)
        {
            var serviceUser = _config.GetValue<string>("ServiceUser2");
            var servicePassword = _config.GetValue<string>("ServicePassword2");
            return Task.FromResult(_directoryService.GetUserInfo(serviceUser, servicePassword, username, employeeNumber));
        }
    }
}
