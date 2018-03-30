using fnf.Client.Client;
using log4net;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;

namespace Tests.IntegrationTests
{
    [TestFixture]
    class Integration
    {
        private Mock<ILog> logMock;
        private Mock<IModel> channelMock;
        private Mock<QueueRegistry> queueRegistryMock;
        private Mock<ChannelRegistry> channelRegistryMock;
        private Mock<ConnectionFactory> connectionFactoryMock;

        [SetUp]
        public void SetUp()
        {
            logMock = new Mock<ILog>();
            channelMock = new Mock<IModel>();
            queueRegistryMock = new Mock<QueueRegistry>();
            channelRegistryMock = new Mock<ChannelRegistry>();
            connectionFactoryMock = new Mock<ConnectionFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            logMock = null;
            channelMock = null;
            queueRegistryMock = null;
            channelRegistryMock = null;
            connectionFactoryMock = null;
        }

        [Test]
        public void GivenPilotApiAndRabbitClient_WhenHostNameIsOkAndServerIsRunning_ThenConnectionCreationIsCalled()
        {
            var rabbitClient = new RabbitClient("Team-0", "Team-0", new ChannelRegistry(), new QueueRegistry(), connectionFactoryMock.Object);
            rabbitClient.Log = logMock.Object;

            var pilotApi = new PilotApi(rabbitClient,"Team-0","Team-0");
            pilotApi.ConnectToRabbitMq();
 
            connectionFactoryMock.Verify(connectionFactory => connectionFactory.CreateConnection());
        }

        [Test]
        public void GivenPilotApiAndRabbitClient_WhenConnectionIsCreatedAndSubscribeCalled_ThenSubscriptionIsBeingCreated()
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            var rabbitClient = new RabbitClient("Team-0", "Team-0",channelRegistryMock.Object, queueRegistryMock.Object, connectionFactory);
            rabbitClient.Log = logMock.Object;
            
            var pilotApi = new PilotApi(rabbitClient, "Team-0", "Team-0");
            pilotApi.ConnectToRabbitMq();
            pilotApi.SubscribeOnSensor((msg) => { });

            logMock.Verify(mock => mock.Info("Subscribed to sensor messages."));
        }

        [Test]
        public void GivenPilotApiAndRabbitClient_WhenConnectionIsCreatedAndPublishCalled_ThenChannelExchangeDeclareIsCalled()
       {
            ChannelRegistry channelRegistry;
           var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            var rabbitClient = new RabbitClient("Team-0", "Team-0", channelRegistry = new ChannelRegistry(), new QueueRegistry(), connectionFactory);
            rabbitClient.Log = logMock.Object;

            var pilotApi = new PilotApi(rabbitClient, "Team-0", "Team-0");
            pilotApi.ConnectToRabbitMq();
            channelRegistry.Channels["Team-0" + "-" + RoutingKeyNames.Power] = channelMock.Object;

            pilotApi.SetPower(120);

            channelMock.Verify(mock => mock.ExchangeDeclare(It.IsAny<string>(), "direct", false, false, null));
        }
    }
}
