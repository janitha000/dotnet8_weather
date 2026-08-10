using '../main.bicep'

param location = 'eastus'
param prefix = 'interview'
param citiesImage = 'mcr.microsoft.com/dotnet/samples:aspnetapp'
param weatherImage = 'mcr.microsoft.com/dotnet/samples:aspnetapp'

// Keep false until you accept APIM Developer SKU cost/time
param deployApim = false
param apimPublisherEmail = 'you@example.com'
param apimPublisherName = 'Interview Demo'
