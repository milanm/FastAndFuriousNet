using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client
{
    public interface IRabbitClient
    {
        void Connect();

        void Disconnect();

        void Subscribe<T>(Action<T> onMessageReceived);

        void Publish(string exchangeName, string routingKey, string message);

        void Publish(string queueName, string message);
    }
}
