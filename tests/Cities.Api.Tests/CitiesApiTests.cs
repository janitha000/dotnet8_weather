using System.Net;

public class CitiesApiTests : IClassFixture<InterviewApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CitiesApiTests(InterviewApiWebApplicationFactory factory)
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
