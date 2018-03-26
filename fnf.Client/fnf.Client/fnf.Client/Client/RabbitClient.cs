using fnf.Client.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client
{
    public class RabbitClient : IRabbitClient
    {
        private IConnection connection;
        private ISerializer serializer = new Serializer();

        private QueueRegistry queueRegistry = new QueueRegistry();
        private ChannelRegistry channelRegistry = new ChannelRegistry();

        private readonly string teamName;
        private readonly string hostName;

        private object publishLock = new object();

        private Dictionary<Type, EventingBasicConsumer> queueConsumers = new Dictionary<Type, EventingBasicConsumer>();

        private Dictionary<Type, string> channelMap = new Dictionary<Type, string>
        {
            [typeof(VelocityMessage)] = RoutingKeyNames.VELOCITY,
            [typeof(SensorMessage)] = RoutingKeyNames.SENSOR,
            [typeof(PenaltyMessage)] = RoutingKeyNames.PENALTY,
            [typeof(RoundTimeMessage)] = RoutingKeyNames.ROUND_PASSED,
            [typeof(StartMessage)] = RoutingKeyNames.START,
            [typeof(StopMessage)] = RoutingKeyNames.STOP
        };

        public RabbitClient(string teamName, string hostName)
        {
            this.teamName = teamName;
            this.hostName = hostName;
        }

        public void Connect()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory { HostName = hostName };
            connection = connectionFactory.CreateConnection();
        }

        public void Subscribe<T>(Action<T> onMessageReceived)
        {
            var routingKey = channelMap[typeof(T)];
            var channel = channelRegistry.GetOrCreateChannel(teamName, routingKey, connection);
            var queueName = queueRegistry.GetOrCreateBindingQueue(teamName, routingKey, channel);

            var consumerExists = queueConsumers.ContainsKey(typeof(T));

            EventingBasicConsumer consumer;
            if (!consumerExists)
            {
                consumer = new EventingBasicConsumer(channel);
                queueConsumers[typeof(T)] = consumer;
            }
            else
            {
                consumer = queueConsumers[typeof(T)];
            }

            consumer.Received += (model, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body);
                T msg = serializer.Deserialize<T>(body);

                onMessageReceived(msg);
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        public void Disconnect()
        {
            if (connection.IsOpen)
                connection.Abort();
        }

        public void Publish(string exchangeName, string routingKey, string message)
        {
            if (connection.IsOpen)
            {
                var channel = channelRegistry.GetOrCreateChannel(exchangeName, routingKey, connection);

                channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchangeName,
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
            }
        }

        public void Publish(string queueName, string message)
        {
            if (connection.IsOpen)
            {
                var channel = channelRegistry.GetOrCreateChannel(queueName, connection);

                channel.QueueDeclare(queue: queueName, durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
