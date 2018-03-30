using RabbitMQ.Client;

namespace fnf.Client.Client
{
    public interface IChannelRegistry
    {
        IModel GetOrCreateChannel(string exchangeName, string routingKey, IConnection connection);

        IModel GetOrCreateChannel(string queueName, IConnection connection);
    }
}
