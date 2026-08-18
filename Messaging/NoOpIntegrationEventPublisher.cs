public sealed class NoOpIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly ILogger<NoOpIntegrationEventPublisher> _logger;

    public NoOpIntegrationEventPublisher(ILogger<NoOpIntegrationEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(string type, string payload, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Outbox no-op publish {Type} ({Length} bytes)", type, payload.Length);
        return Task.CompletedTask;
    }
}
