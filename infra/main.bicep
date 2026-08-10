targetScope = 'resourceGroup'

@description('Azure region for all resources')
param location string = resourceGroup().location

@description('Short name prefix, e.g. interview')
@minLength(3)
@maxLength(12)
param prefix string = 'interview'

@description('Container image for Cities API (ACR or public placeholder)')
param citiesImage string = 'mcr.microsoft.com/dotnet/samples:aspnetapp'

@description('Container image for Weather API')
param weatherImage string = 'mcr.microsoft.com/dotnet/samples:aspnetapp'

@description('APIM is costly/slow — keep false until you intentionally deploy it')
param deployApim bool = false

@description('APIM publisher email (required when deployApim is true)')
param apimPublisherEmail string = 'you@example.com'

@description('APIM publisher name')
param apimPublisherName string = 'Interview Demo'

// ---------- shared logging (required by Container Apps Environment) ----------
module logAnalytics 'modules/logAnalytics.bicep' = {
  name: 'log-analytics'
  params: {
    name: 'log-${prefix}'
    location: location
  }
}

// ---------- Container Apps Environment (CAE) ----------
module cae 'modules/containerAppsEnvironment.bicep' = {
  name: 'cae'
  params: {
    name: 'cae-${prefix}'
    location: location
    logAnalyticsCustomerId: logAnalytics.outputs.customerId
    logAnalyticsSharedKey: logAnalytics.outputs.primarySharedKey
  }
}

// ---------- Cities API (InterviewApi) ----------
module citiesApi 'modules/containerApp.bicep' = {
  name: 'cities-api'
  params: {
    name: 'ca-${prefix}-cities'
    location: location
    environmentId: cae.outputs.id
    image: citiesImage
    targetPort: 8080
    externalIngress: true
    envVars: [
      {
        name: 'ASPNETCORE_URLS'
        value: 'http://+:8080'
      }
      // Wire real values later from Key Vault / app settings:
      // { name: 'ConnectionStrings__Default', secretRef: 'sql-conn' }
      // { name: 'JWT__Key', secretRef: 'jwt-key' }
    ]
  }
}

// ---------- Weather API ----------
module weatherApi 'modules/containerApp.bicep' = {
  name: 'weather-api'
  params: {
    name: 'ca-${prefix}-weather'
    location: location
    environmentId: cae.outputs.id
    image: weatherImage
    targetPort: 8080
    externalIngress: true
    envVars: [
      {
        name: 'ASPNETCORE_URLS'
        value: 'http://+:8080'
      }
      // East-west: point at Cities FQDN inside the CAE (set after first deploy or via param)
      // { name: 'CitiesApi__LookupMode', value: 'Http' }
      // { name: 'CitiesApi__BaseUrl', value: 'https://${citiesApi.outputs.fqdn}' }
      // { name: 'CitiesApi__GrpcUrl', value: 'https://${citiesApi.outputs.fqdn}' }
    ]
  }
}

// ---------- APIM (optional placeholder for local YARP) ----------
module apim 'modules/apim.bicep' = if (deployApim) {
  name: 'apim'
  params: {
    name: 'apim-${prefix}'
    location: location
    publisherEmail: apimPublisherEmail
    publisherName: apimPublisherName
    citiesBackendUrl: 'https://${citiesApi.outputs.fqdn}'
    weatherBackendUrl: 'https://${weatherApi.outputs.fqdn}'
  }
}

output containerAppsEnvironmentId string = cae.outputs.id
output citiesFqdn string = citiesApi.outputs.fqdn
output weatherFqdn string = weatherApi.outputs.fqdn
output apimGatewayUrl string = deployApim ? apim.outputs.gatewayUrl : ''
