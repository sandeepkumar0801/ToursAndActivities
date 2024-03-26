using Autofac;
using ServiceAdapters.SightSeeing.SightSeeing.Commands;
using ServiceAdapters.SightSeeing.SightSeeing.Commands.Contracts;
using ServiceAdapters.SightSeeing.SightSeeing.Converters;
using ServiceAdapters.SightSeeing.SightSeeing.Converters.Contract;

namespace ServiceAdapters.SightSeeing
{
    public class SightSeeingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IssuingCommandHandler>().As<IIssuingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ConfirmCommandHandler>().As<IConfirmCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<IssuingConverter>().As<IIssuingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CancelTicketCommandHandler>().As<ICancelTicketCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CancelTicketConverter>().As<ICancelTicketConverter>().InstancePerLifetimeScope();
        }
    }
}