using System.Collections.Generic;
using RabbitMQ.Client; 

namespace fnf.Client.Client
{
    class ChannelRegistry
    {
        private Dictionary<string, IModel> channels = new Dictionary<string, IModel>();

        public IModel GetOrCreateChannel(string exchangeName, string routingKey, IConnection connection)
        {
            IModel result;
            if (channels.ContainsKey(exchangeName + "-" + routingKey))
            {
               result = channels[exchangeName + "-" + routingKey];
            }
            else
            {
                var channel = connection.CreateModel();
                result = channels[exchangeName + "-" + routingKey] = channel; 
            }
            return result;
        }

        public IModel GetOrCreateChannel(string queueName, IConnection connection)
        {
            IModel result;
            if (channels.ContainsKey(queueName))
            {
                result = channels[queueName];
            }
            else
            {
                var channel = connection.CreateModel();
                result = channels[queueName] = channel;
            }
            return result;
        }
    }
}
