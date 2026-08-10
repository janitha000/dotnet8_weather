@description('APIM placeholder — maps to local GatewayApi/YARP routes.')
param name string
param location string
param publisherEmail string
param publisherName string

@description('https://<cities-container-app-fqdn>')
param citiesBackendUrl string

@description('https://<weather-container-app-fqdn>')
param weatherBackendUrl string

// Developer SKU is for demos only (cost + long create time).
resource apim 'Microsoft.ApiManagement/service@2023-09-01-preview' = {
  name: name
  location: location
  sku: {
    name: 'Developer'
    capacity: 1
  }
  properties: {
    publisherEmail: publisherEmail
    publisherName: publisherName
  }
}

// --- Backends (like YARP clusters) ---
resource citiesBackend 'Microsoft.ApiManagement/service/backends@2023-09-01-preview' = {
  parent: apim
  name: 'cities-backend'
  properties: {
    protocol: 'http'
    url: citiesBackendUrl
    description: 'InterviewApi / Cities (local :5249)'
  }
}

resource weatherBackend 'Microsoft.ApiManagement/service/backends@2023-09-01-preview' = {
  parent: apim
  name: 'weather-backend'
  properties: {
    protocol: 'http'
    url: weatherBackendUrl
    description: 'WeatherApi (local :5250)'
  }
}

// --- APIs (like YARP routes) ---
resource citiesApi 'Microsoft.ApiManagement/service/apis@2023-09-01-preview' = {
  parent: apim
  name: 'cities-api'
  properties: {
    displayName: 'Cities & Auth'
    path: ''
    protocols: [
      'https'
    ]
    subscriptionRequired: false
  }
}

resource weatherApi 'Microsoft.ApiManagement/service/apis@2023-09-01-preview' = {
  parent: apim
  name: 'weather-api'
  properties: {
    displayName: 'Weather'
    path: ''
    protocols: [
      'https'
    ]
    subscriptionRequired: false
  }
}

// Operations ≈ /api/cities/** and /api/weather/** (fill policies in portal or add policy resources later)
resource citiesOp 'Microsoft.ApiManagement/service/apis/operations@2023-09-01-preview' = {
  parent: citiesApi
  name: 'cities-catch-all'
  properties: {
    displayName: 'Cities catch-all'
    method: 'GET'
    urlTemplate: '/api/cities/{*path}'
    templateParameters: [
      {
        name: 'path'
        type: 'string'
        required: false
      }
    ]
  }
}

resource weatherOp 'Microsoft.ApiManagement/service/apis/operations@2023-09-01-preview' = {
  parent: weatherApi
  name: 'weather-catch-all'
  properties: {
    displayName: 'Weather catch-all'
    method: 'GET'
    urlTemplate: '/api/weather/{*path}'
    templateParameters: [
      {
        name: 'path'
        type: 'string'
        required: false
      }
    ]
  }
}

// Placeholder policies: set-backend-service (complete in portal or next iteration)
resource citiesOpPolicy 'Microsoft.ApiManagement/service/apis/operations/policies@2023-09-01-preview' = {
  parent: citiesOp
  name: 'policy'
  properties: {
    format: 'xml'
    value: '''
      <policies>
        <inbound>
          <base />
          <set-backend-service backend-id="cities-backend" />
        </inbound>
        <backend><base /></backend>
        <outbound><base /></outbound>
        <on-error><base /></on-error>
      </policies>
    '''
  }
}

resource weatherOpPolicy 'Microsoft.ApiManagement/service/apis/operations/policies@2023-09-01-preview' = {
  parent: weatherOp
  name: 'policy'
  properties: {
    format: 'xml'
    value: '''
      <policies>
        <inbound>
          <base />
          <set-backend-service backend-id="weather-backend" />
        </inbound>
        <backend><base /></backend>
        <outbound><base /></outbound>
        <on-error><base /></on-error>
      </policies>
    '''
  }
}

output name string = apim.name
output gatewayUrl string = apim.properties.gatewayUrl
