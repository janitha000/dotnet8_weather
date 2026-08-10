using Cities.Grpc;
using Grpc.Core;

public class CityLookupGrpcService : CityLookup.CityLookupBase
{
    private readonly ICityRepository _cities;
    private readonly ICityNormalizer _normalizer;

    public CityLookupGrpcService(ICityRepository cities, ICityNormalizer normalizer)
    {
        _cities = cities;
        _normalizer = normalizer;
    }

    public override async Task<FindCityByNameReply> FindByName(
        FindCityByNameRequest request,
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return new FindCityByNameReply { Found = false };

        var city = await _cities.GetByNameAsync(
            _normalizer.Normalize(request.Name),
            context.CancellationToken);

        if (city is null)
            return new FindCityByNameReply { Found = false };

        return new FindCityByNameReply
        {
            Found = true,
            Name = city.Name,
            Country = city.Country,
            Latitude = city.Latitude,
            Longitude = city.Longitude
        };
    }
}
