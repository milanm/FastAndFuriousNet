using fnf.Client.Client;
using System;
using System.Reflection;
using Ninject;

namespace fnf.Client
{
    class Program
    { 
        static void Main()
        {
            IKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
        
            IPilotApi pilotApi = kernel.Get<IPilotApi>();

            var pilot = new Pilot.Pilot(pilotApi);
            
            pilotApi.ConnectToRabbitMq();

            pilot.SubscribeToAllChannels();

            Console.ReadKey();
        }
    }
}
