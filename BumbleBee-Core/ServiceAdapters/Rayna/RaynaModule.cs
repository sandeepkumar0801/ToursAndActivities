using Autofac;
using ServiceAdapters.Rayna.Rayna.Commands;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Converters;
using ServiceAdapters.Rayna.Rayna.Converters.Contracts;

namespace ServiceAdapters.RaynaModule
{
    public class RaynaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Register Command Handlers
            builder.RegisterType<RaynaCityCmdHandler>().As<IRaynaCityCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaCountryCmdHandler>().As<IRaynaCountryCmdHandler>().InstancePerLifetimeScope();

            builder.RegisterType<RaynaTourStaticDataCmdHandler>().As<IRaynaTourStaticDataCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaTourStaticDatabyIdCmdHandler>().As<IRaynaTourStaticDataByIdCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaTourOptionCmdHandler>().As<IRaynaTourOptionCmdHandler>().InstancePerLifetimeScope();

            builder.RegisterType<RaynaAvailabilityTourOptionCmdHandler>().As<IRaynaAvailabilityTourOptionCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaAvailabilityTimeSlotCmdHandler>().As<IRaynaAvailabilityTimeSlotCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaAvailabilityCmdHandler>().As<IRaynaAvailabilityCmdHandler>().InstancePerLifetimeScope();


            builder.RegisterType<RaynaAvailabilityConverter>().As<IRaynaAvailabilityConverter>().InstancePerLifetimeScope();

            builder.RegisterType<RaynaBookingCmdHandler>().As<IRaynaBookingCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaBookingConverter>().As<IRaynaBookingConverter>().InstancePerLifetimeScope();

            builder.RegisterType<RaynaTourTicketCmdHandler>().As<IRaynaTourTicketCmdHandler>().InstancePerLifetimeScope();

            builder.RegisterType<RaynaCancelCmdHandler>().As<IRaynaCancelCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaCancelConverter>().As<IRaynaCancelConverter>().InstancePerLifetimeScope();
        }
    }
}