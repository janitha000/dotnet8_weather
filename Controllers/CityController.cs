using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ServiceFilter(typeof(LogActionFilter))]
[ApiController]
[Route("api/cities")]
public class CityController : ControllerBase
{
    // Kept for reference: pre-MediatR / service-layer style
    private readonly ICityService _cityService;
    private readonly ISender _sender;

    public CityController(ICityService cityService, ISender sender)
    {
        _cityService = cityService;
        _sender = sender;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetCityByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        // Reference (service approach):
        // var city = await _cityService.GetCityByNameAsync(name, cancellationToken);

        // Current (CQRS / MediatR query):
        var city = await _sender.Send(new GetCityByNameQuery(name), cancellationToken);
        if (city == null) return NotFound("City not found");
        return Ok(city);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(City), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<City>> CreateCityAsync(
        [FromBody] CreateCityDto createCityDto,
        CancellationToken cancellationToken = default)
    {
        // Reference (service approach):
        // var city = new City
        // {
        //     Name = createCityDto.Name,
        //     Country = createCityDto.Country,
        //     Latitude = createCityDto.Latitude,
        //     Longitude = createCityDto.Longitude,
        //     TimeZone = createCityDto.TimeZone
        // };
        // city = await _cityService.CreateCityAsync(city, cancellationToken);

        // Current (CQRS / MediatR command):
        var city = await _sender.Send(
            new CreateCityCommand(
                createCityDto.Name,
                createCityDto.Country,
                createCityDto.Latitude,
                createCityDto.Longitude,
                createCityDto.TimeZone),
            cancellationToken);

        return CreatedAtAction(nameof(GetCityByNameAsync), new { name = city.Name }, city);
    }
}
