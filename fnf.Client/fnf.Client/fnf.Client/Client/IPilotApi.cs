using System;
using fnf.Client.Messages;

namespace fnf.Client.Client
{
    interface IPilotApi
    { 
        void SetPower(int powerMessage);

        void SubscribeOnRaceStart(Action<StartMessage> startMessageAction);

        void SubscribeOnRaceEnd(Action<StopMessage> stopMessageAction);

        void SubscribeOnVelocity(Action<VelocityMessage> velocityMessageAction);

        void SubscribeOnSensor(Action<SensorMessage> sensorMessageAction);

        void SubscribeOnRoundPassed(Action<RoundTimeMessage> roundTimeMessageAction);

        void SubscribeOnPenalty(Action<PenaltyMessage> penaltyMessageAction);

        void ConnectToRabbitMq();

        void DisconnectFromRabbitMq();
    }
}
