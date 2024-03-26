using Autofac;
using Logger.Contract;

namespace Logger
{
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Logger>().As<ILogger>().InstancePerLifetimeScope();
        }
    }
}