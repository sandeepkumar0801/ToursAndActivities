using Autofac;
using ServiceAdapters.PrioHub.PrioHub.Commands;
using ServiceAdapters.PrioHub.PrioHub.Commands.Contract;
using ServiceAdapters.PrioHub.PrioHub.Converters;
using ServiceAdapters.PrioHub.PrioHub.Converters.Contracts;

namespace ServiceAdapters.PrioHub
{
    public class PrioHubModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProductsCmdHandler>().As<IProductsCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RouteCommandHandler>().As<IRouteCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AvailabilityListCmdHandler>().As<IAvailabilityListCommandHandler>()
               .InstancePerLifetimeScope();

            builder.RegisterType<ProductDetailCmdHandler>().As<IProductDetailCommandHandler>()
               .InstancePerLifetimeScope();

            builder.RegisterType<AvailablityListConverter>().As<IAvailablityListConverter>()
              .InstancePerLifetimeScope();

            builder.RegisterType<ProductDetailConverter>().As<IProductDetailConverter>()
              .InstancePerLifetimeScope();

            //Booking
            builder.RegisterType<ReservationCmdHandler>().As<IReservationCommandHandler>()
              .InstancePerLifetimeScope();

            builder.RegisterType<ReservationConverter>().As<IReservationConverter>()
              .InstancePerLifetimeScope();

            builder.RegisterType<CreateBookingCmdHandler>().As<ICreateBookingCommandHandler>()
              .InstancePerLifetimeScope();

            builder.RegisterType<CreateBookingConverter>().As<ICreateBookingConverter>()
              .InstancePerLifetimeScope();


            //Cancellation
            builder.RegisterType<CancelReservationCmdHandler>().As<ICancelReservationCmdHandler>()
             .InstancePerLifetimeScope();

            builder.RegisterType<CancelReservationConverter>().As<ICancelReservationConverter>()
              .InstancePerLifetimeScope();

            builder.RegisterType<CancelBookingCmdHandler>().As<ICancelBookingCmdHandler>()
              .InstancePerLifetimeScope();

            builder.RegisterType<CancelBookingConverter>().As<ICancelBookingConverter>()
              .InstancePerLifetimeScope();

            builder.RegisterType<GetVoucherCmdHandler>().As<IGetVoucherCommandHandler>()
              .InstancePerLifetimeScope();
        }
    }
}