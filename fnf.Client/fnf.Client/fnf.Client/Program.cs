using fnf.Client.Client;
using fnf.Client.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using log4net;

namespace fnf.Client
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        static void Main(string[] args)
        {
            log.Info("First log");

            var rabbit = new RabbitClient("Team-0", "localhost");

            var serializer = new Serializer();

            var pilotApi = new PilotApi(rabbit);

            rabbit.Connect();

            var pilot = new Pilot(pilotApi,"Team-0");

            pilot.SubscribeToAllChannels();

            Console.ReadKey();
        }
    }
}
