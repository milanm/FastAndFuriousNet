using fnf.Client.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace fnf.Client.Client
{
    public class PilotApi : IPilotApi
    {
        private IRabbitClient client;

        private ISerializer serializer = new Serializer();

        private Thread publishingThread;

        private object powerLock = new object(); 

        public PilotApi(IRabbitClient client)
        {
            this.client = client;
        }

        public void AnnounceIsAlive(KeepAliveMessage keepAliveMessage)
        {
            var message = serializer.Serialize<KeepAliveMessage>(keepAliveMessage);
            client.Publish(RoutingKeyNames.ANNOUNCE,message);
        }


        public void SetPower(PowerMessage powerMessage)
        {  
            var exchangeName = powerMessage.TeamId;
            var message = serializer.Serialize<PowerMessage>(powerMessage);
            lock (powerLock)
            {
                client.Publish(exchangeName, RoutingKeyNames.POWER, message);
            }     
        }

        public void SubscribeOnRaceStart(Action<StartMessage> startMessageAction)
        {
            client.Subscribe<StartMessage>(startMessageAction);
        }

        public void SubscribeOnRaceEnd(Action<StopMessage> stopMessageAction)
        {
            client.Subscribe<StopMessage>(stopMessageAction);
        }

        public void SubscribeOnVelocity(Action<VelocityMessage> velocityMessageAction)
        {
            client.Subscribe<VelocityMessage>(velocityMessageAction);
        }

        public void SubscribeOnSensor(Action<SensorMessage> sensorMessageAction)
        {
            client.Subscribe(sensorMessageAction);
        }

        public void SubscribeOnRoundPassed(Action<RoundTimeMessage> roundTimeMessageAction)
        {
            client.Subscribe<RoundTimeMessage>(roundTimeMessageAction);
        }

        public void SubscribeOnPenalty(Action<PenaltyMessage> penaltyMessageAction)
        {
           client.Subscribe<PenaltyMessage>(penaltyMessageAction);
        }
    }
}
