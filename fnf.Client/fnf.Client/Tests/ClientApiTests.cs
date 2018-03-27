using System;
using Moq;
using NUnit.Framework;
using fnf.Client;
using fnf.Client.Client;
using fnf.Client.Serialization;

namespace Tests
{
    [TestFixture]
    public class ClientApiTests
    {
        private Mock<IRabbitClient> rabbitClientMock;

        [SetUp]
        public void SetUp()
        {
            rabbitClientMock = new Mock<IRabbitClient>();
        }

        [TearDown]
        public void TearDown()
        {
            rabbitClientMock = null;
        }

        [Test]
        public void GivenRabbitClient_WhenPowerMessageSent_ThenRabbitReceivesAMessage()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object);
            var serializer = new Serializer();

            var message = new PowerMessage()
            {
                AccessCode = "Team-0",
                P = 110,
                TeamId = "Team-1",
                TimeStamp = 12345
            };

            pilotApi.SetPower(message);

            rabbitClientMock.Verify(mock => mock.Publish(message.TeamId, RoutingKeyNames.Power, serializer.Serialize(message)));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToVelocity_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object);
            pilotApi.SubscribeOnVelocity((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<VelocityMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToSensor_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object);
            pilotApi.SubscribeOnSensor((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<SensorMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToPenalty_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object);
            pilotApi.SubscribeOnPenalty((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<PenaltyMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToRoundEnd_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object);
            pilotApi.SubscribeOnRoundPassed((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<RoundTimeMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToStart_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object);
            pilotApi.SubscribeOnRaceStart((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<StartMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToStop_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object);
            pilotApi.SubscribeOnRaceEnd((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<StopMessage>>()));
        }
    }
}
