using fnf.Client.Client;
using fnf.Client.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client
{
    class Pilot
    {
        private IPilotApi pilotApi;

        private ISerializer serializer = new Serializer();

        private Action<VelocityMessage> velocityHandler;

        private Action<SensorMessage> sensorHandler;

        private Action<PenaltyMessage> penaltyHandler;

        private Action<RoundTimeMessage> roundEndHandler;

        private Action<StartMessage> startHandler;

        private Action<StopMessage> stopHandler;

        private string accessCode; 

        public const int SAFE_POWER = 120;

        public const double SAFE_GYRO = 1000;

        public const double DANGEROUS_GYRO = 4000;

        private int currentPower = SAFE_POWER;

        private double lastGyroZ = 0; 

        private double currentGyroZ = 0;

        public void OnVelocityMessage(VelocityMessage velocityMessage)
        {
          
        }

        public void OnSensorMessage(SensorMessage sensorMessage)
        {
            lastGyroZ = currentGyroZ;

            currentGyroZ = sensorMessage.G[2];

            if ((currentGyroZ > lastGyroZ) && ((DANGEROUS_GYRO - currentGyroZ) > (currentGyroZ - SAFE_GYRO)))
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

            Console.WriteLine("Gyro Z is " + currentGyroZ);
        }

        public void OnPenaltyMessage(PenaltyMessage penaltyMessage)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            lastGyroZ = currentGyroZ;

            currentPower = SAFE_POWER; 

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

            Console.WriteLine(serializer.Serialize<RoundTimeMessage>(roundTimeMessage));
        }

        public void OnStartMessage(StartMessage startMessage)
        {
            Console.WriteLine("Start message received");
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
            pilotApi.SubscribeOnVelocity(velocityHandler = OnVelocityMessage);
            pilotApi.SubscribeOnSensor(sensorHandler = OnSensorMessage);
            pilotApi.SubscribeOnPenalty(penaltyHandler = OnPenaltyMessage);
            pilotApi.SubscribeOnRoundPassed(roundEndHandler = OnRoundEndMessage);
            pilotApi.SubscribeOnRaceStart(startHandler = OnStartMessage);
            pilotApi.SubscribeOnRaceEnd(stopHandler = OnStopMessage); 
        }


    }
}
