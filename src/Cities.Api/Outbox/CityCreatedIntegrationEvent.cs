public record CityCreatedIntegrationEvent(
    Guid EventId,
    int CityId,
    string Name,
    string Country,
    string TenantId,
    DateTime OccurredOnUtc);