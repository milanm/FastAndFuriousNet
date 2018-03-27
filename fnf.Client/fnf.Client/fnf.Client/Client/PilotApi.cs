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

        public PilotApi(IRabbitClient client)
        {
            this.client = client;
        }

        public void AnnounceIsAlive(KeepAliveMessage keepAliveMessage)
        {
            var message = serializer.Serialize(keepAliveMessage);
            client.Publish(RoutingKeyNames.Announce,message);
        }


        public void SetPower(PowerMessage powerMessage)
        {  
            var exchangeName = powerMessage.TeamId;
            var message = serializer.Serialize(powerMessage);
            lock (powerLock)
            {
                client.Publish(exchangeName, RoutingKeyNames.Power, message);
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
    }
}
