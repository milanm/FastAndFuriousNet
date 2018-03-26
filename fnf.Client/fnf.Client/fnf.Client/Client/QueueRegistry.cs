using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client; 

namespace fnf.Client
{
    class QueueRegistry
    {
        private Dictionary<string, string> queueNames = new Dictionary<string, string>();

        public string GetOrCreateBindingQueue(string exchange, string routingKey, IModel channel)
        {
            string queueName; 
            if (queueNames.ContainsKey(exchange + "-" + routingKey))
            {
                queueName = queueNames[exchange + "-" + routingKey];
            }
            else
            {
                channel.ExchangeDeclare(exchange: exchange, type: "direct");
                queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);
                queueNames[exchange + "-" + routingKey] = queueName;
            }
            return queueName;
        }
    }
}
