using System;
using fnf.Client.Client;
using RabbitMQ.Client;
using log4net;
using NUnit.Framework;
using Moq;

namespace Tests.UnitTests
{
    [TestFixture(Category = "Unit Tests")]
    class RabbitClientTests
    {
        private Mock<ILog> logMock = new Mock<ILog>();
        private Mock<IChannelRegistry> channelRegistryMock = new Mock<IChannelRegistry>();
        private Mock<IQueueRegistry> queueRegistryMock = new Mock<IQueueRegistry>(); 

        [SetUp]
        public void SetUp()
        {
            logMock = new Mock<ILog>();
            channelRegistryMock = new Mock<IChannelRegistry>();
            queueRegistryMock = new Mock<IQueueRegistry>();
        }

        [TearDown]
        public void TearDown()
        {
            logMock = null;
            channelRegistryMock = null;
            queueRegistryMock = null; 
        }

        [Test]
        public void GivenRabbitClient_WhenConnectionCreationCalledWithBadParameters_ConnectionFailed()
        {
            var connectionFactory = new ConnectionFactory() { HostName = "badHostAddres" };

            RabbitClient rabbitClient = new RabbitClient("Team-0","Team-0", channelRegistryMock.Object, queueRegistryMock.Object, connectionFactory);
            rabbitClient.Log = logMock.Object;
            rabbitClient.Connect();

            logMock.Verify((mock) => mock.Error(It.IsAny<string>()));
        }

        [Test]
        public void GivenRabbitClient_WhenPublishToQueueCalledWithClosedConnection_PublishFails()
        {
            var connectionFactory = new ConnectionFactory() { HostName = "badHostAddres" };

            RabbitClient rabbitClient = new RabbitClient("Team-0","Team-0", channelRegistryMock.Object, queueRegistryMock.Object, connectionFactory);
            rabbitClient.Log = logMock.Object;
            rabbitClient.Connect();

            rabbitClient.Publish(It.IsAny<string>(), It.IsAny<string>());
            logMock.Verify(mock => mock.Info("Can not publish because connection is not created or is closed"));
        }

        [Test]
        public void GivenRabbitClient_WhenPublishToExchangeCalledWithClosedConnection_PublishFails()
        {
            var connectionFactory = new ConnectionFactory() { HostName = "badHostAddres" };

            RabbitClient rabbitClient = new RabbitClient("Team-0","Team-0", channelRegistryMock.Object, queueRegistryMock.Object,connectionFactory);
            rabbitClient.Log = logMock.Object;
            rabbitClient.Connect();

            rabbitClient.Publish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            logMock.Verify(mock => mock.Info("Can not publish because connection is not created or is closed"));
        }


        [Test]
        public void GivenRabbitClient_WhenSubscribeCalledWithClosedConnection_SubscribeFails()
        {
            var connectionFactory = new ConnectionFactory() { HostName = "badHostAddres" };

            RabbitClient rabbitClient = new RabbitClient("Team-0", "Team-0", channelRegistryMock.Object, queueRegistryMock.Object, connectionFactory);
            rabbitClient.Log = logMock.Object;
            rabbitClient.Subscribe(It.IsAny<Action<object>>());

            logMock.Verify(mock =>
                mock.Info("Can not subscribe to channel because connection is not created or is closed"));
        }
    }
}
