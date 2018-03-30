using RabbitMQ.Client; 

namespace fnf.Client.Client
{
    public interface IQueueRegistry
    {
        string GetOrCreateBindingQueue(string exchange, string routingKey, IModel channel);
    }
}
