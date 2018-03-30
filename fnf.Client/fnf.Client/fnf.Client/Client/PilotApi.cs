using fnf.Client.Serialization;
using fnf.Client.Messages;
using System;

namespace fnf.Client.Client
{
    public class PilotApi : IPilotApi
    {
        private IRabbitClient client;

        private ISerializer serializer = new Serializer();

        private object powerLock = new object();

        private string accessCode;

        private string teamName; 

        public PilotApi(IRabbitClient client, string accessCode, string teamName)
        {
            this.client = client;
            this.accessCode = accessCode;
            this.teamName = teamName; 
        }

        public void SetPower(int power)
        {
            var milliseconds = (new DateTime(1970, 1, 1) - new DateTime()).Milliseconds;
            var powerMessage = new PowerMessage(){ AccessCode = accessCode, P = power, TeamId = teamName, TimeStamp = milliseconds };
           
            var message = serializer.Serialize(powerMessage);

            lock (powerLock)
            {
                client.Publish(teamName, RoutingKeyNames.Power, message);
            }     
        }

        public void SubscribeOnRaceStart(Action<StartMessage> startMessageAction)
        {
            client.Subscribe(startMessageAction);
        }

        public void SubscribeOnRaceEnd(Action<StopMessage> stopMessageAction)
        {
            client.Subscribe(stopMessageAction);
        }

        public void SubscribeOnVelocity(Action<VelocityMessage> velocityMessageAction)
        {
            client.Subscribe(velocityMessageAction);
        }

        public void SubscribeOnSensor(Action<SensorMessage> sensorMessageAction)
        {
            client.Subscribe(sensorMessageAction);
        }

        public void SubscribeOnRoundPassed(Action<RoundTimeMessage> roundTimeMessageAction)
        {
            client.Subscribe(roundTimeMessageAction);
        }

        public void SubscribeOnPenalty(Action<PenaltyMessage> penaltyMessageAction)
        {
           client.Subscribe(penaltyMessageAction);
        }

        public void ConnectToRabbitMq()
        {
            client.Connect(); 
        }

        public void DisconnectFromRabbitMq()
        {
            client.Disconnect();
        }
    }
}
