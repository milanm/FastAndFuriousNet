using fnf.Client.Messages;

namespace fnf.Client.Pilot
{
    interface IPilot
    {
        void OnVelocityMessage(VelocityMessage velocityMessage);

        void OnSensorMessage(SensorMessage sensorMessage);

        void OnPenaltyMessage(PenaltyMessage penaltyMessage);

        void OnRoundEndMessage(RoundTimeMessage roundTimeMessage);

        void OnStartMessage(StartMessage startMessage);

        void OnStopMessage(StopMessage stopMessage);

        void SetPower(byte power);

        void SubscribeToAllChannels();
    }
}
