using 'main.bicep'

param namePrefix = 'covidpass'
param entraClientId = readEnvironmentVariable('COVIDPASS_ENTRA_CLIENT_ID', '00000000-0000-0000-0000-000000000000')
param sqlAdminLogin = readEnvironmentVariable('COVIDPASS_SQL_ADMIN_LOGIN', 'covidpass-sql-admins')
param sqlAdminObjectId = readEnvironmentVariable('COVIDPASS_SQL_ADMIN_OBJECT_ID', '00000000-0000-0000-0000-000000000000')
