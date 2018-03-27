using fnf.Client.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using fnf.Client.Messages;

namespace fnf.Client.Client
{
    public class RabbitClient : IRabbitClient
    {
        private IConnection connection;
        private ISerializer serializer = new Serializer();

        private QueueRegistry queueRegistry = new QueueRegistry();
        private ChannelRegistry channelRegistry = new ChannelRegistry();

        private readonly string teamName;
        private readonly string hostName;

        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<Type, EventingBasicConsumer> queueConsumers = new Dictionary<Type, EventingBasicConsumer>();

        private Dictionary<Type, string> channelMap = new Dictionary<Type, string>
        {
            [typeof(VelocityMessage)] = RoutingKeyNames.Velocity,
            [typeof(SensorMessage)] = RoutingKeyNames.Sensor,
            [typeof(PenaltyMessage)] = RoutingKeyNames.Penalty,
            [typeof(RoundTimeMessage)] = RoutingKeyNames.RoundPassed,
            [typeof(StartMessage)] = RoutingKeyNames.Start,
            [typeof(StopMessage)] = RoutingKeyNames.Stop
        };

        public RabbitClient(string teamName, string hostName)
        {
            this.teamName = teamName;
            this.hostName = hostName;
        }

        public void Connect()
        {
            log.Info("Establishing the connection with rabbitMQ server");
            try
            {
                ConnectionFactory connectionFactory = new ConnectionFactory {HostName = hostName};
                connection = connectionFactory.CreateConnection();
            }
            catch (Exception e)
            {
                log.Error("Failed to connect to rabbitMQ server");
                log.Error("Error details : " + e.Message);
                return; 
            }
            log.Info("Connection established");
        }

        public void Subscribe<T>(Action<T> onMessageReceived)
        {
            if (connection != null && connection.IsOpen)
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
                    log.Info(serializer.Serialize(msg));
                };

                channel.BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);
            }
            else
            {
                log.Error("Can not subscribe to channel because connection is not created or is closed.");
            }
        }

        public void Disconnect()
        {
            if (connection.IsOpen)
            {
                connection.Abort();
                log.Info("Connection aborted.");
            }
        }

        public void Publish(string exchangeName, string routingKey, string message)
        {
            if (connection != null && connection.IsOpen)
            {
                var channel = channelRegistry.GetOrCreateChannel(exchangeName, routingKey, connection);

                channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchangeName,
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
            }
            else
            {
                log.Info("Can not publish because connection is not created or is closed");
            }
        }

        public void Publish(string queueName, string message)
        {
            if (connection != null && connection.IsOpen)
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
            else
            {
                log.Info("Can not publish because connection is not created or is closed");
            }
        }
    }
}
