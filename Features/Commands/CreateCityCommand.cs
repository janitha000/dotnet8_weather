using MediatR;

public record CreateCityCommand(
    string Name,
    string Country,
    string Latitude,
    string Longitude,
    string TimeZone
) : IRequest<City>;