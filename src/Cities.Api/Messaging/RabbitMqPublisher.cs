using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

public sealed class RabbitMqPublisher : IIntegrationEventPublisher, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly object _gate = new();

    private IConnection? _connection;
    private IModel? _channel;
    private bool _disposed;

    public RabbitMqPublisher(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishAsync(string type, string payload, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();

        var channel = EnsureChannel();
        var body = Encoding.UTF8.GetBytes(payload);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = type;
        properties.DeliveryMode = 2;

        if (type == nameof(CityCreatedIntegrationEvent))
        {
            var evt = System.Text.Json.JsonSerializer.Deserialize<CityCreatedIntegrationEvent>(payload);
            if (!string.IsNullOrWhiteSpace(evt?.TenantId))
            {
                properties.Headers = new Dictionary<string, object>
                {
                    [TenantClaims.TenantId] = evt.TenantId
                };
            }
        }

        var routingKey = type == nameof(CityCreatedIntegrationEvent)
            ? RabbitMqTopology.CityCreatedRoutingKey
            : type.ToLowerInvariant();

        channel.BasicPublish(
            exchange: RabbitMqTopology.Exchange,
            routingKey: routingKey,
            basicProperties: properties,
            body: body);

        channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));

        _logger.LogInformation("Published {Type} to {Exchange} ({RoutingKey})",
            type, RabbitMqTopology.Exchange, routingKey);

        return Task.CompletedTask;
    }

    private IModel EnsureChannel()
    {
        lock (_gate)
        {
            if (_channel is { IsOpen: true })
                return _channel;

            _channel?.Dispose();
            _connection?.Dispose();

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(5),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            RabbitMqTopology.Declare(_channel);
            _channel.ConfirmSelect();

            _logger.LogInformation("RabbitMQ channel ready on {Host}:{Port}",
                _options.HostName, _options.Port);

            return _channel;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _channel?.Dispose();
        _connection?.Dispose();
    }
}