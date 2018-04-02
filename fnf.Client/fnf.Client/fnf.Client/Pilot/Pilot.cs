using fnf.Client.Client;
using fnf.Client.Messages;

namespace fnf.Client.Pilot
{
    class Pilot : IPilot
    {
        private IPilotApi pilotApi;

        public const byte SafePower = 110;

        public void OnVelocityMessage(VelocityMessage velocityMessage)
        {
            pilotApi.SetPower(SafePower);
        }

        public void OnSensorMessage(SensorMessage sensorMessage)
        {
            pilotApi.SetPower(SafePower);
        }

        public void OnPenaltyMessage(PenaltyMessage penaltyMessage)
        {
            pilotApi.SetPower(SafePower);
        }

        public void OnRoundEndMessage(RoundTimeMessage roundTimeMessage)
        {
            pilotApi.SetPower(SafePower);
        }

        public void OnStartMessage(StartMessage startMessage)
        {
            pilotApi.SetPower(SafePower);
        }

        public void OnStopMessage(StopMessage stopMessage)
        {
            pilotApi.SetPower(0);
        }
        
        public Pilot(IPilotApi pilotApi)
        {
            this.pilotApi = pilotApi;
        }

        public void SetPower(byte power)
        {
            pilotApi.SetPower(power);
        }

        public void SubscribeToAllChannels()
        {
            pilotApi.SubscribeOnVelocity(OnVelocityMessage);
            pilotApi.SubscribeOnSensor(OnSensorMessage);
            pilotApi.SubscribeOnPenalty(OnPenaltyMessage);
            pilotApi.SubscribeOnRoundPassed(OnRoundEndMessage);
            pilotApi.SubscribeOnRaceStart(OnStartMessage);
            pilotApi.SubscribeOnRaceEnd(OnStopMessage); 
        }
    }
}
