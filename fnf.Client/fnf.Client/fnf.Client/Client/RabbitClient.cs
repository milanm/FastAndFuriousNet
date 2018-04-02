using fnf.Client.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using fnf.Client.Messages;
using log4net;

namespace fnf.Client.Client
{
    public class RabbitClient : IRabbitClient
    {
        private IConnection connection;
        public IConnection Connection
        {
            get { return connection; }
        }

        private ISerializer serializer = new Serializer();

        private IQueueRegistry queueRegistry;
        private IChannelRegistry channelRegistry;
        private IConnectionFactory connectionFactory;

        private readonly string accessCode;
        private readonly string teamName;

        private Thread announceAliveThread;
        private object announceAliveLock = new object();
        private volatile bool announcingAllowed;

        public ILog Log { get; set; }

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

        public RabbitClient(string teamName,string accessCode,IChannelRegistry channelRegistry,IQueueRegistry queueRegistry,IConnectionFactory connectionFactory)
        {
            this.teamName = teamName;
            this.accessCode = accessCode;
            this.channelRegistry = channelRegistry;
            this.queueRegistry = queueRegistry;
            this.connectionFactory = connectionFactory;
            Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void Connect()
        {
            Log.Info("Establishing the connection with rabbitMQ server");
            try
            {
                connection = connectionFactory.CreateConnection();
            }
            catch (Exception e)
            {
                Log.Error("Failed to connect to rabbitMQ server");
                Log.Error("Error details : " + e.Message);
                return; 
            }
            Log.Info("Connection established");
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

                    if (channelMap[typeof(T)] == RoutingKeyNames.Start)
                        InitializeAnnouncingAliveThread();

                    if (channelMap[typeof(T)] == RoutingKeyNames.Stop)
                    {
                        lock (announceAliveLock)
                        {
                            announcingAllowed = false;
                        }
                    }

                    onMessageReceived(msg);
                    Log.Info(serializer.Serialize(msg));
                };

                channel.BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);

                Log.Info("Subscribed to " + channelMap[typeof(T)] + " messages.");
            }
            else
            {
                Log.Info("Can not subscribe to channel because connection is not created or is closed");
            }
        }

        public void Disconnect()
        {
            if (connection.IsOpen)
            {
                connection.Abort();
                Log.Info("Connection aborted.");
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
                Log.Info("Can not publish because connection is not created or is closed");
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
                Log.Info("Can not publish because connection is not created or is closed");
            }
        }

        private void InitializeAnnouncingAliveThread()
        {
            announceAliveThread = new Thread(() =>
            {
                lock (announceAliveLock)
                {
                    announcingAllowed = true; 
                }

                Log.Info("Created AnnounceIsAlive thread");

                var timeStamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Milliseconds;

                try
                {
                    while (announcingAllowed)
                    {
                        KeepAliveMessage message = new KeepAliveMessage()
                        {
                            AccessCode = accessCode,
                            OptionalUrl = "",
                            TeamId = teamName,
                            TimeStamp = timeStamp
                        };

                        Publish(teamName,RoutingKeyNames.Announce,serializer.Serialize(message));
                        Thread.Sleep(1000);
                    }
                    Log.Info("Terminating AnnounceIsAlive thread");
                }
                catch (Exception e)
                {
                    Log.Info(e.Message);
                }
            });

            announceAliveThread.Start();
        }
    }
}
