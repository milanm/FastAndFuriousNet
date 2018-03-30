using fnf.Client.Client;
using Ninject.Modules;
using RabbitMQ.Client;

namespace fnf.Client.DependencyInjection
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IQueueRegistry>().To<QueueRegistry>();

            Bind<IChannelRegistry>().To<ChannelRegistry>();

            Bind<IPilotApi>().To<PilotApi>().WithConstructorArgument("teamName", "Team-0")
                .WithConstructorArgument("accessCode", "Team-0");

            Bind<IConnectionFactory>().To<ConnectionFactory>()
                .WithConstructorArgument("HostName", "localhost");

            Bind<IRabbitClient>().To<RabbitClient>().WithConstructorArgument("teamName", "Team-0")
                .WithConstructorArgument("accessCode", "Team-0");
        }
    }
}
