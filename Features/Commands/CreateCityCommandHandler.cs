using MediatR;

public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, City>
{
    private readonly ICityRepository _cityRepository;
    private readonly ICityNormalizer _cityNormalizer;
    private readonly IPublisher _publisher;
    private readonly ILogger<CreateCityCommandHandler> _logger;

    public CreateCityCommandHandler(
        ICityRepository cityRepository,
        ICityNormalizer cityNormalizer,
        IPublisher publisher,
        ILogger<CreateCityCommandHandler> logger)
    {
        _cityRepository = cityRepository;
        _cityNormalizer = cityNormalizer;
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

        await _cityRepository.AddAsync(city, cancellationToken);
        await _cityRepository.SaveChangesAsync(cancellationToken);

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
