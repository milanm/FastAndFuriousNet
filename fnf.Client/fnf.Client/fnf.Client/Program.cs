using fnf.Client.Client;
using System;
using fnf.Client.Pilot;
using log4net;
using RabbitMQ.Client;

namespace fnf.Client
{
    class Program
    { 
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Not all command line arguments provided.\nPlease provide accessCode and TeamId.\nPress any key to continue...");
                Console.ReadKey();
                return; 
            }

            var serverUrl = args[0]; 
            var accessCode = args[1];
            var teamName = args[2];

            IConnectionFactory connectionFactory = new ConnectionFactory() {HostName = serverUrl};
            IRabbitClient rabbitClient = new RabbitClient(teamName,accessCode,new ChannelRegistry(), new QueueRegistry(), connectionFactory);
            IPilotApi pilotApi = new PilotApi(rabbitClient, accessCode, teamName);
            IPilot pilot = new Pilot.Pilot(pilotApi);
            
            pilotApi.ConnectToRabbitMq();
            pilot.SubscribeToAllChannels();

            Console.ReadKey();
        }
    }
}
