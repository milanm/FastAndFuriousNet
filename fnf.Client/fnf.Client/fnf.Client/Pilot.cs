using fnf.Client.Client;
using System;
using fnf.Client.Messages;

namespace fnf.Client
{
    class Pilot
    {
        private IPilotApi pilotApi;

        private string accessCode; 

        public const int SafePower = 120;

        public const double SafeGyro = 1000;

        public const double DangerousGyro = 4000;

        private int currentPower = SafePower;

        private double lastGyroZ;

        private double currentGyroZ;

        public void OnVelocityMessage(VelocityMessage velocityMessage)
        {
          
        }

        public void OnSensorMessage(SensorMessage sensorMessage)
        {
            lastGyroZ = currentGyroZ;

            currentGyroZ = sensorMessage.G[2];

            if ((currentGyroZ > lastGyroZ) && ((DangerousGyro - currentGyroZ) > (currentGyroZ - SafeGyro)))
            {
                currentPower -= 5;
            }
            else
            {
                currentPower += 5;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            SetPower(new PowerMessage()
            {
                AccessCode = accessCode,
                P = currentPower,
                TeamId = accessCode,
                TimeStamp = Convert.ToInt64((DateTime.Now - epoch).TotalMilliseconds)
            });

            
        }

        public void OnPenaltyMessage(PenaltyMessage penaltyMessage)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            lastGyroZ = currentGyroZ;

            currentPower = SafePower; 

            SetPower(new PowerMessage()
            {
                AccessCode = accessCode,
                P = currentPower,
                TeamId = accessCode,
                TimeStamp = Convert.ToInt64((DateTime.Now - epoch).TotalMilliseconds)
            });
        }

        public void OnRoundEndMessage(RoundTimeMessage roundTimeMessage)
        {
           var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            currentPower += 5;

            SetPower(new PowerMessage()
            {
                AccessCode = accessCode,
                P = currentPower,
                TeamId = accessCode,
                TimeStamp = Convert.ToInt64((DateTime.Now - epoch).TotalMilliseconds)
            });
        }

        public void OnStartMessage(StartMessage startMessage)
        {
            
        }

        public void OnStopMessage(StopMessage stopMessage)
        {
               
        }
        
        public Pilot(IPilotApi pilotApi, string accessCode)
        {
            this.pilotApi = pilotApi;
            this.accessCode = accessCode;
        }

        public void AnnounceIsAlive(KeepAliveMessage keepAliveMessage)
        {
            pilotApi.AnnounceIsAlive(keepAliveMessage);
        }

        public void SetPower(PowerMessage powerMessage)
        {
            pilotApi.SetPower(powerMessage);
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
