using Autofac;
using ServiceAdapters.PrioTicket.PrioTicket.Commands;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Converters;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;

namespace ServiceAdapters.PrioTicket
{
    public class PrioTicketsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AvailablityCmdHandler>()
                .As<IAvailablityCmdHandler>().InstancePerDependency();
            builder.RegisterType<CancelBookingCmdHandler>()
                .As<ICancelBookingCmdHandler>().InstancePerDependency();
            builder.RegisterType<CancelReservationCmdHandler>()
                .As<ICancelReservationCmdHandler>().InstancePerDependency();
            builder.RegisterType<CreateBookingCmdHandler>()
                .As<ICreateBookingCmdHandler>().InstancePerDependency();
            builder.RegisterType<ReservationCmdHandler>()
                .As<IReservationCmdHandler>().InstancePerDependency();
            builder.RegisterType<TicketDetailsCmdHandler>()
                .As<ITicketDetailsCmdHandler>().InstancePerDependency();
            builder.RegisterType<UpdateBookingCmdHandler>()
                .As<IUpdateBookingCmdHandler>().InstancePerDependency();
            builder.RegisterType<TicketDetailsCmdHandler>()
                .As<ITicketDetailsCmdHandler>().InstancePerDependency();
            builder.RegisterType<TicketListCmdHandler>()
               .As<ITicketListCmdHandler>().InstancePerDependency();

            builder.RegisterType<AvailablityConverter>()
                .As<IAvailablityConverter>().InstancePerDependency();
            builder.RegisterType<CancelBookingConverter>()
                .As<ICancelBookingConverter>().InstancePerDependency();
            builder.RegisterType<CancelReservationConverter>()
                .As<ICancelReservationConverter>().InstancePerDependency();
            builder.RegisterType<CreateBookingConverter>()
                .As<ICreateBookingConverter>().InstancePerDependency();
            builder.RegisterType<ReservationConverter>()
                .As<IReservationConverter>().InstancePerDependency();
            builder.RegisterType<UpdateBookingConverter>()
                .As<IUpdateBookingConverter>().InstancePerDependency();
            builder.RegisterType<TicketDetailsConverter>()
                .As<ITicketDetailsConverter>().InstancePerDependency();
            builder.RegisterType<TicketListConverter>()
              .As<ITicketListConverter>().InstancePerDependency();
        }
    }
}