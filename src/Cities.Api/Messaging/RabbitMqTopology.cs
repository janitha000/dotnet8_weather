using RabbitMQ.Client;

public static class RabbitMqTopology
{
    public const string Exchange = "interview.events";
    public const string DeadLetterExchange = "interview.events.dlx";
    public const string CityCreatedRoutingKey = "city.created";
    public const string WeatherCityCreatedQueue = "weather.city-created";
    public const string WeatherCityCreatedDlq = "weather.city-created.dlq";

    public static void Declare(IModel channel)
    {
        channel.ExchangeDeclare(Exchange, ExchangeType.Topic, durable: true);
        channel.ExchangeDeclare(DeadLetterExchange, ExchangeType.Fanout, durable: true);

        var queueArgs = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = DeadLetterExchange
        };

        channel.QueueDeclare(
            WeatherCityCreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs);
        channel.QueueBind(WeatherCityCreatedQueue, Exchange, CityCreatedRoutingKey);

        channel.QueueDeclare(
            WeatherCityCreatedDlq,
            durable: true,
            exclusive: false,
            autoDelete: false);
        channel.QueueBind(WeatherCityCreatedDlq, DeadLetterExchange, routingKey: string.Empty);
    }
}
