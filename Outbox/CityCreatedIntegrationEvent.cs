public record CityCreatedIntegrationEvent(
    Guid EventId,
    int CityId,
    string Name,
    string Country,
    DateTime OccurredOnUtc);