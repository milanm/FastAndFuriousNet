using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client.Client
{
    interface IPilotApi
    {
        void AnnounceIsAlive(KeepAliveMessage keepAliveMessage);

        void SetPower(PowerMessage powerMessage);

        void SubscribeOnRaceStart(Action<StartMessage> startMessageAction);

        void SubscribeOnRaceEnd(Action<StopMessage> stopMessageAction);

        void SubscribeOnVelocity(Action<VelocityMessage> velocityMessageAction);

        void SubscribeOnSensor(Action<SensorMessage> sensorMessageAction);

        void SubscribeOnRoundPassed(Action<RoundTimeMessage> roundTimeMessageAction);

        void SubscribeOnPenalty(Action<PenaltyMessage> penaltyMessageAction);
    }
}
