using Autofac;
using ServiceAdapters.HB.HB.Commands;
using ServiceAdapters.HB.HB.Commands.Contract;
using ServiceAdapters.HB.HB.Converters;
using ServiceAdapters.HB.HB.Converters.Contracts;

namespace ServiceAdapters.HB
{
    public class HbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HBBookingCancelCmdHandler>().As<IHbBookingCancelCmdHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<HBBookingCancelSimulationCmdHandler>().As<IHbBookingCancelSimulationCmdHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<HBBookingConfirmCmdHandler>().As<IHbBookingConfirmCmdHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<HBDeatilCmdHandler>().As<IHbDeatilCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<HBDetailFullCmdHandler>().As<IHbDetailFullCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<HBCalendarCmdHandler>().As<IHBCalendarCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<HBContentMultiCmdHandler>().As<IHBContentMultiCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<HBGetBookingDetailCmdHandler>().As<IHbGetBookingDetailCmdHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<HBSearchCmdHandler>().As<IHbSearchCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<HBBookingCancelConverter>().As<IHbBookingCancelConverter>().InstancePerLifetimeScope();
            builder.RegisterType<HBBookingCancelSimulationConverter>().As<IHbBookingCancelSimulationConverter>()
                .InstancePerLifetimeScope();
            builder.RegisterType<HBBookingConfirmConverter>().As<IHbBookingConfirmConverter>()
                .InstancePerLifetimeScope();
            builder.RegisterType<HBDetailConverter>().As<IHbDetailConverter>().InstancePerLifetimeScope();
            builder.RegisterType<HBDetailFullConverter>().As<IHbDetailFullConverter>().InstancePerLifetimeScope();
            builder.RegisterType<HBGetBookingDetailConverter>().As<IHbGetBookingDetailConverter>()
                .InstancePerLifetimeScope();
            builder.RegisterType<HBSearchConverter>().As<IHbSearchConverter>().InstancePerLifetimeScope();
            builder.RegisterType<HBCalendarConverter>().As<IHbCalendarConverter>().InstancePerLifetimeScope();
        }
    }
}