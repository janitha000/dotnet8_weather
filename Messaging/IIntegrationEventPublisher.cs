public interface IIntegrationEventPublisher
{
    Task PublishAsync(string type, string payload, CancellationToken cancellationToken);
}
