using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

public class CitiesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CitiesApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCity_ReturnsUnauthorized_WhenNoToken()
    {
        var response = await _client.GetAsync("/api/cities/NoSuchCityXYZ");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
