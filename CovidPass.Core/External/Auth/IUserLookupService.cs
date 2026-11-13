using System.Threading;
using System.Threading.Tasks;

namespace CovidPass.Core.External.Auth
{
    public interface IUserLookupService
    {
        Task<DirectoryUserInfo> FindUser(string username, string employeeNumber, CancellationToken cancellationToken = default);
    }
}
