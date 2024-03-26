using Autofac;
using ServiceAdapters.GoldenTours.GoldenTours.Commands;
using ServiceAdapters.GoldenTours.GoldenTours.Commands.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Converters;
using ServiceAdapters.GoldenTours.GoldenTours.Converters.Contracts;

namespace ServiceAdapters.GoldenTours
{
    public class GoldenToursModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region CommandHandlers

            builder.RegisterType<ProductDetailsCommandHandler>()
                .As<IProductDetailsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<AvailabilityCommandHandler>().
                As<IAvailabilityCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<BookingCommandHandler>().
                As<IBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetBookingDatesCommandHandler>().
                As<IGetBookingDatesCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetProductDatesCommandHandler>().
                As<IGetProductDatesCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<PickupPointsCommandHandler>().
                As<IPickupPointsCommandHandler>().InstancePerLifetimeScope();

            #endregion CommandHandlers

            #region Converters

            builder.RegisterType<ProductDetailsConverter>()
                .As<IProductDetailsConverter>().InstancePerLifetimeScope();
            builder.RegisterType<AvailabilityConverter>()
                .As<IAvailabilityConverter>().InstancePerLifetimeScope();
            builder.RegisterType<BookingConverter>()
                .As<IBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetBookingDatesConverter>()
                .As<IGetBookingDatesConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetProductDatesConverter>()
                .As<IGetProductDatesConverter>().InstancePerLifetimeScope();
            builder.RegisterType<PickupPointsConverter>()
                .As<IPickupPointsConverter>().InstancePerLifetimeScope();

            #endregion Converters

            #region Adapter

            builder.RegisterType<GoldenToursAdapter>().
                As<IGoldenToursAdapter>().InstancePerLifetimeScope();

            #endregion Adapter
        }
    }
}