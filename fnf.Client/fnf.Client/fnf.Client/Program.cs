using fnf.Client.Client;
using System;

namespace fnf.Client
{
    class Program
    {
       


        static void Main()
        {
            var rabbit = new RabbitClient("Team-0", "localhost");

            var pilotApi = new PilotApi(rabbit);

            rabbit.Connect();

            var pilot = new Pilot(pilotApi,"Team-0");

            pilot.SubscribeToAllChannels();

            Console.ReadKey();
        }
    }
}
