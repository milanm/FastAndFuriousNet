using fnf.Client.Client;
using fnf.Client.Messages;

namespace fnf.Client.Pilot
{
    class Pilot : IPilot
    {
        private IPilotApi pilotApi;

        public const int SafePower = 110;

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

        public void SetPower(int power)
        {
            pilotApi.SetPower(power);
        }

        public void SubscribeToAllChannels()
        {
            pilotApi.SubscribeOnVelocity((message) => { OnVelocityMessage(message);});
            pilotApi.SubscribeOnSensor((message) => { OnSensorMessage(message);});
            pilotApi.SubscribeOnPenalty((message) => { OnPenaltyMessage(message);});
            pilotApi.SubscribeOnRoundPassed((message) => { OnRoundEndMessage(message);});
            pilotApi.SubscribeOnRaceStart((message) => { OnStartMessage(message);});
            pilotApi.SubscribeOnRaceEnd((message) => { OnStopMessage(message);}); 
        }
    }
}
