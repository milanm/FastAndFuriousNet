using System;
using Moq;
using NUnit.Framework;
using fnf.Client.Messages;
using fnf.Client.Client;

namespace Tests.UnitTests
{
    [TestFixture(Category = "Unit Tests")]
    public class PilotApiTests
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
            var pilotApi = new PilotApi(rabbitClientMock.Object, "Team-0", "Team-0");

            pilotApi.SetPower(100);

            rabbitClientMock.Verify(mock => mock.Publish(It.IsAny<string>(), RoutingKeyNames.Power, It.IsAny<string>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToVelocity_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object, "Team-0", "Team-0");
            pilotApi.SubscribeOnVelocity((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<VelocityMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToSensor_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object, "Team-0", "Team-0");
            pilotApi.SubscribeOnSensor((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<SensorMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToPenalty_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object, "Team-0", "Team-0");
            pilotApi.SubscribeOnPenalty((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<PenaltyMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToRoundEnd_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object, "Team-0", "Team-0");
            pilotApi.SubscribeOnRoundPassed((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<RoundTimeMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToStart_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object, "Team-0", "Team-0");
            pilotApi.SubscribeOnRaceStart((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<StartMessage>>()));
        }

        [Test]
        public void GivenRabbitClient_WhenSubscribedToStop_ThenReceivesMessages()
        {
            var pilotApi = new PilotApi(rabbitClientMock.Object, "Team-0", "Team-0");
            pilotApi.SubscribeOnRaceEnd((message) => { });
            rabbitClientMock.Verify(mock => mock.Subscribe(It.IsAny<Action<StopMessage>>()));
        }
    }
}
