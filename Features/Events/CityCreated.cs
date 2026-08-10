using MediatR;

public record CityCreated(
    int Id,
    string Name,
    string Country,
    string Latitude,
    string Longitude,
    string TimeZone
) : INotification;