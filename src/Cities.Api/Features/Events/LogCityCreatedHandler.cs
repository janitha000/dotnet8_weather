using MediatR;

public class LogCityCreatedHandler : INotificationHandler<CityCreated>
{
    private readonly ILogger<LogCityCreatedHandler> _logger;

    public LogCityCreatedHandler(ILogger<LogCityCreatedHandler> logger) => _logger = logger;

    public Task Handle(CityCreated notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain event CityCreated: {CityId} {CityName} {TenantId}",
            notification.Id,
            notification.Name,
            notification.TenantId);
        return Task.CompletedTask;
    }
}