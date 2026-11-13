using System.Threading.Tasks;
using CovidPass.Core.Infrastructure.Models;
using CovidPass.Core.Models.Config;

namespace CovidPass.Core.Modules.Auth.GetAuthSettings
{
    public class GetAuthSettingsHandler : QueryHandler<GetAuthSettingsQuery, AuthSettings>
    {
        private readonly AuthConfigModel _authConfig;

        public GetAuthSettingsHandler(AuthConfigModel authConfig)
        {
            _authConfig = authConfig;
        }

        public override Task<AuthSettings> Handle(GetAuthSettingsQuery model)
        {
            if (!_authConfig.IsEntraId)
                return Task.FromResult(new AuthSettings { Mode = AuthModes.Directory });

            var entra = _authConfig.EntraId;
            return Task.FromResult(new AuthSettings
            {
                Mode = AuthModes.EntraId,
                ClientId = entra.ClientId,
                Authority = $"{entra.Instance.TrimEnd('/')}/{entra.TenantId}",
                Scopes = entra.EffectiveScopes
            });
        }
    }
}
