using System.Text.Json;
using MediatR;

public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, City>
{
    private readonly ICityRepository _cityRepository;
    private readonly ICityNormalizer _cityNormalizer;
    private readonly AppDbContext _db;
    private readonly IPublisher _publisher;
    private readonly ILogger<CreateCityCommandHandler> _logger;

    public CreateCityCommandHandler(
        ICityRepository cityRepository,
        ICityNormalizer cityNormalizer,
        AppDbContext db,
        IPublisher publisher,
        ILogger<CreateCityCommandHandler> logger)
    {
        _cityRepository = cityRepository;
        _cityNormalizer = cityNormalizer;
        _db = db;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<City> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = _cityNormalizer.Normalize(request.Name);
        if (await _cityRepository.ExistsByNameAsync(normalizedName, cancellationToken))
        {
            _logger.LogWarning("Duplicate city create attempt {CityName}", request.Name);
            throw new DuplicateException($"City with name '{request.Name}' already exists");
        }

        var city = new City
        {
            Name = request.Name,
            Country = request.Country,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            TimeZone = request.TimeZone
        };

        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

        await _cityRepository.AddAsync(city, cancellationToken);
        await _cityRepository.SaveChangesAsync(cancellationToken); // Id assigned

        var integration = new CityCreatedIntegrationEvent(
            Guid.NewGuid(),
            city.Id,
            city.Name,
            city.Country,
            DateTime.UtcNow);

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = integration.EventId,
            Type = nameof(CityCreatedIntegrationEvent),
            Payload = JsonSerializer.Serialize(integration),
            OccurredOnUtc = integration.OccurredOnUtc
        });

        await _db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        // Local domain event after commit (cache/log)
        await _publisher.Publish(
            new CityCreated(
                city.Id,
                city.Name,
                city.Country,
                city.Latitude,
                city.Longitude,
                city.TimeZone),
            cancellationToken);

        return city;
    }
}
