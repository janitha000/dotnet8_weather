using System.Net;
using System.Net.Http.Json;

public class CityLookupHttpClient : ICityLookup
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CityLookupHttpClient> _logger;

    public CityLookupHttpClient(HttpClient httpClient, ILogger<CityLookupHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CityLocation?> FindByNameAsync(
        string cityName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            return null;

        var path = $"api/cities/{Uri.EscapeDataString(cityName.Trim())}";
        using var response = await _httpClient.GetAsync(path, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Cities API failed with {StatusCode} for {City}",
                response.StatusCode,
                cityName);
            throw new AppException(
                StatusCodes.Status502BadGateway,
                $"Failed to look up city '{cityName}'");
        }

        return await response.Content.ReadFromJsonAsync<CityLocation>(cancellationToken: cancellationToken);
    }
}
