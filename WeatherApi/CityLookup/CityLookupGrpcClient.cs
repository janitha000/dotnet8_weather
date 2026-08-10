using Cities.Grpc;
using Grpc.Core;

public class CityLookupGrpcClient : ICityLookup
{
    private readonly CityLookup.CityLookupClient _client;
    private readonly ILogger<CityLookupGrpcClient> _logger;

    public CityLookupGrpcClient(
        CityLookup.CityLookupClient client,
        ILogger<CityLookupGrpcClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<CityLocation?> FindByNameAsync(
        string cityName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            return null;

        try
        {
            var reply = await _client.FindByNameAsync(
                new FindCityByNameRequest { Name = cityName.Trim() },
                cancellationToken: cancellationToken);

            if (!reply.Found)
                return null;

            return new CityLocation
            {
                Name = reply.Name,
                Country = reply.Country,
                Latitude = reply.Latitude,
                Longitude = reply.Longitude
            };
        }
        catch (RpcException ex)
        {
            _logger.LogWarning(ex, "Cities gRPC lookup failed for {City}", cityName);
            throw new AppException(
                StatusCodes.Status502BadGateway,
                $"Failed to look up city '{cityName}' via gRPC");
        }
    }
}
