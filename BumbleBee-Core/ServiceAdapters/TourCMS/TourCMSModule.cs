using Autofac;
using ServiceAdapters.TourCMS.TourCMS.Commands;
using ServiceAdapters.TourCMS.TourCMS.Commands.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Converters;
using ServiceAdapters.TourCMS.TourCMS.Converters.Contracts;

namespace ServiceAdapters.TourCMS
{
    public class TourCMSModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Register Command Handlers
            builder.RegisterType<ChannelCommandHandler>().As<IChannelCommandHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ChannelShowCommandHandler>().As<IChannelShowCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TourCommandHandler>().As<ITourCommandHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<TourShowCommandHandler>().As<ITourShowCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DatesnDealsCommandHandler>().As<IDatesnDealsCommandHandler>()
             .InstancePerLifetimeScope();

            builder.RegisterType<DatesnDealsConverter>().As<IDatesnDealsConverter>()
              .InstancePerLifetimeScope();

            builder.RegisterType<CheckAvailabilityCommandHandler>().As<ICheckAvailabilityCommandHandler>()
             .InstancePerLifetimeScope();

            builder.RegisterType<AvailabilityConverter>().As<IAvailabilityConverter>()
              .InstancePerLifetimeScope();


            builder.RegisterType<NewBookingCommandHandler>().As<INewBookingCommandHandler>()
             .InstancePerLifetimeScope();

            builder.RegisterType<CommitBookingCommandHandler>().As<ICommitBookingCommandHandler>()
              .InstancePerLifetimeScope();


            builder.RegisterType<NewBookingConverter>().As<INewBookingConverter>()
              .InstancePerLifetimeScope();

            builder.RegisterType<CommitBookingConverter>().As<ICommitBookingConverter>()
              .InstancePerLifetimeScope();


            builder.RegisterType<CancelBookingCommandHandler>().As<ICancelBookingCommandHandler>()
            .InstancePerLifetimeScope();

            builder.RegisterType<CancelBookingConverter>().As<ICancelBookingConverter>()
              .InstancePerLifetimeScope();
            
            builder.RegisterType<DeleteBookingConverter>().As<IDeleteBookingConverter>()
             .InstancePerLifetimeScope();

            builder.RegisterType<DeleteBookingCommandHandler>().As<IDeleteBookingCommandHandler>()
          .InstancePerLifetimeScope();

            builder.RegisterType<TourCMSRedemptionCommandHandler>().As<ITourCMSRedemptionCommandHandler>()
             .InstancePerLifetimeScope();

        }
    }
}