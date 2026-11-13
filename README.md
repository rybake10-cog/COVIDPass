# COVID Pass
COVID Pass is not a substitute for professional medical advice, diagnosis, or treatment. It is only for screening related to COVID-19. 

This screening aid is based on recommendations from the Centers for Disease Control and Prevention, the Massachusetts Department of Public Health, and Partners HealthCare experts. Screening criteria in other states may be different.

Always consult a medical professional when needed.

To be functional, the application will require access to the following:
- A service to authenticate users
- A service to get additional information about the authenticated user
- A backend database to save user attestation data

#### Please read the license agreement before proceeding. ####



## Running on Azure

The app now targets .NET 10 and runs as a Linux container on Azure Container Apps, with:

| Concern | Before | Now |
|---|---|---|
| Runtime | .NET Core 3.1 on Windows/IIS | .NET 10 container (`Dockerfile`), also still runs on IIS |
| Sign-in | Username/password checked against on-prem AD (`DirectoryService`), app-issued JWT | Microsoft Entra ID (MSAL redirect in the SPA, API validates Entra access tokens) |
| User lookup | LDAP with a service account | Microsoft Graph via the app's managed identity |
| Database | SQL Server with SQL login | Azure SQL with Entra-only auth via managed identity |
| Secrets | `appsettings.json` | Azure Key Vault (`KeyVault:Uri`), no secrets in config |
| Infra / CD | Manual | `infra/main.bicep`, `.github/workflows/deploy.yml` |

The sign-in mode is chosen by `Auth:Mode`: `EntraId` (default in the container) or `Directory` (original AD/LDAP flow, Windows only).

### Configuration

| Setting | Purpose |
|---|---|
| `Auth:Mode` | `EntraId` or `Directory` |
| `Auth:EntraId:TenantId`, `Auth:EntraId:ClientId` | Entra ID app registration |
| `Auth:EntraId:Audience` | API audience; defaults to `api://<ClientId>` |
| `Auth:EntraId:LoginClaim`, `EmployeeIdClaim`, `DepartmentClaim` | Token claims mapped to the user's login, employee ID and department |
| `KeyVault:Uri` | Load secrets from Key Vault; secret `Auth--Key` maps to `Auth:Key` |
| `Connections:Default` | SQL connection string; on Azure uses `Authentication=Active Directory Managed Identity` |
| `Database:MigrateOnStartup` | Run FluentMigrator migrations on start (default `true`) |
| `Directory:*` | AD/LDAP host, domain and DNs for `Directory` mode |

### One-time Azure setup

1. **App registration** (Entra ID): add a SPA platform with redirect URI `https://<app-host>/employee/`, expose an API with scope `access_as_user`, and add `employeeid` and `department` claims to the access token (claims-mapping policy or directory extension). Set `Auth:EntraId:EmployeeIdClaim`/`DepartmentClaim` if you use different claim names.
2. **Deployment identity**: create a GitHub OIDC federated credential with Contributor and User Access Administrator on the resource group; set the repository variables listed in `.github/workflows/deploy.yml`.
3. **Run** the *Deploy to Azure* workflow. The first run provisions infrastructure, builds the image in ACR and deploys the Container App.
4. **Database access**: as the Entra SQL admin, run `infra/grant-app-access.sql` with the `identityName` output.
5. **Graph access**: grant the managed identity the Microsoft Graph `User.Read.All` application permission (used by the employee lookup).

### Local development

```bash
dotnet test CovidPass.sln                          # set COVIDPASS_TEST_SQL to also run the SQL Server tests
cd CovidPass.Front && npm ci && NODE_OPTIONS=--openssl-legacy-provider npm run build
docker build -t covidpass .
```
