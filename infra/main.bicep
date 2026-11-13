// COVIDPass on Azure: Container Apps + Azure SQL (Entra-only auth) + Key Vault + ACR, all accessed via one managed identity.
targetScope = 'resourceGroup'

@description('Short prefix used for resource names.')
@minLength(3)
@maxLength(12)
param namePrefix string = 'covidpass'

param location string = resourceGroup().location

@description('Container image, e.g. <acr>.azurecr.io/covidpass:<sha>. Leave empty to provision everything except the app (first run, before an image exists).')
param containerImage string = ''

@description('Application (client) ID of the COVIDPass Entra ID app registration (SPA + exposed API).')
param entraClientId string

@description('Entra ID tenant for sign-in.')
param entraTenantId string = tenant().tenantId

@description('Display name of the Entra ID user or group that administers Azure SQL.')
param sqlAdminLogin string

@description('Object ID of the Entra ID user or group that administers Azure SQL.')
param sqlAdminObjectId string

@allowed(['User', 'Group', 'Application'])
param sqlAdminPrincipalType string = 'Group'

@description('Site names shown in the form.')
param sites array = ['Site 1', 'Site 2']

param emailServer string = ''
param emailFrom string = ''

var suffix = uniqueString(resourceGroup().id)
var names = {
  identity: '${namePrefix}-id'
  logs: '${namePrefix}-logs'
  acr: toLower(replace('${namePrefix}${suffix}', '-', ''))
  keyVault: take('${namePrefix}-kv-${suffix}', 24)
  sqlServer: '${namePrefix}-sql-${suffix}'
  sqlDb: namePrefix
  env: '${namePrefix}-env'
  app: namePrefix
}

var roles = {
  acrPull: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
  keyVaultSecretsUser: '4633458b-17de-408a-b874-0445c86b69e6'
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: names.identity
  location: location
}

resource logs 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: names.logs
  location: location
  properties: {
    sku: { name: 'PerGB2018' }
    retentionInDays: 30
  }
}

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: names.acr
  location: location
  sku: { name: 'Basic' }
  properties: { adminUserEnabled: false }
}

resource acrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(acr.id, identity.id, roles.acrPull)
  scope: acr
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles.acrPull)
    principalId: identity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: names.keyVault
  location: location
  properties: {
    tenantId: tenant().tenantId
    sku: { family: 'A', name: 'standard' }
    enableRbacAuthorization: true
    enableSoftDelete: true
    enablePurgeProtection: true
    softDeleteRetentionInDays: 90
  }
}

resource keyVaultSecretsUser 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, identity.id, roles.keyVaultSecretsUser)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles.keyVaultSecretsUser)
    principalId: identity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: names.sqlServer
  location: location
  properties: {
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      login: sqlAdminLogin
      sid: sqlAdminObjectId
      principalType: sqlAdminPrincipalType
      tenantId: tenant().tenantId
    }
  }
}

resource allowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: names.sqlDb
  location: location
  sku: { name: 'GP_S_Gen5_1', tier: 'GeneralPurpose' }
  properties: {
    autoPauseDelay: 60
    minCapacity: json('0.5')
    requestedBackupStorageRedundancy: 'Local'
  }
}

resource env 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: names.env
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logs.properties.customerId
        sharedKey: logs.listKeys().primarySharedKey
      }
    }
  }
}

var sqlConnectionString = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Database=${names.sqlDb};Authentication=Active Directory Managed Identity;User Id=${identity.properties.clientId};Encrypt=True;Connection Timeout=60'
var siteSettings = [for (site, i) in sites: { name: 'Sites__${i}', value: site }]

resource app 'Microsoft.App/containerApps@2024-03-01' = if (!empty(containerImage)) {
  name: names.app
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: { '${identity.id}': {} }
  }
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
        allowInsecure: false
      }
      registries: [
        {
          server: acr.properties.loginServer
          identity: identity.id
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'covidpass'
          image: containerImage
          resources: { cpu: json('0.5'), memory: '1Gi' }
          env: concat([
            { name: 'AZURE_CLIENT_ID', value: identity.properties.clientId }
            { name: 'KeyVault__Uri', value: keyVault.properties.vaultUri }
            { name: 'Connections__Default', value: sqlConnectionString }
            { name: 'Auth__Mode', value: 'EntraId' }
            { name: 'Auth__EntraId__TenantId', value: entraTenantId }
            { name: 'Auth__EntraId__ClientId', value: entraClientId }
            { name: 'EmailServer', value: emailServer }
            { name: 'EmailFrom', value: emailFrom }
          ], siteSettings)
          probes: [
            {
              type: 'Liveness'
              httpGet: { path: '/api/health', port: 8080 }
              initialDelaySeconds: 10
            }
            {
              type: 'Readiness'
              httpGet: { path: '/api/health', port: 8080 }
            }
          ]
        }
      ]
      scale: { minReplicas: 1, maxReplicas: 3 }
    }
  }
  dependsOn: [acrPull, keyVaultSecretsUser]
}

output appUrl string = empty(containerImage) ? '' : 'https://${app!.properties.configuration.ingress.fqdn}/employee/'
output acrLoginServer string = acr.properties.loginServer
output acrName string = acr.name
output containerAppName string = names.app
output keyVaultName string = keyVault.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output sqlDatabaseName string = sqlDb.name
output identityName string = identity.name
output identityPrincipalId string = identity.properties.principalId
