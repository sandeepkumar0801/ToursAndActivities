using Autofac;
using ServiceAdapters.FareHarbor.FareHarbor.Commands;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Converters;
using ServiceAdapters.FareHarbor.FareHarbor.Converters.Contracts;

namespace ServiceAdapters.FareHarbor
{
    public class FareHarborModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateBookingCommandHandler>().As<ICreateBookingCommandHandler>().InstancePerDependency();

            builder.RegisterType<DeleteBookingCommandHandler>().As<IDeleteBookingCommandHandler>().InstancePerDependency();

            builder.RegisterType<GetAvailabilitiesByDateCommandHandler>().As<IGetAvailabilitiesByDateCommandHandler>().InstancePerDependency();

            builder.RegisterType<GetAvailabilityCommandHandler>().As<IGetAvailabilityCommandHandler>().InstancePerDependency();

            builder.RegisterType<GetAvailabilitiesCommandHandler>().As<IGetAvailabilitiesCommandHandler>().InstancePerDependency();

            builder.RegisterType<GetBookingCommandHandler>().As<IGetBookingCommandHandler>().InstancePerDependency();

            builder.RegisterType<GetCompaniesCommandHandler>().As<IGetCompaniesCommandHandler>().InstancePerDependency();

            builder.RegisterType<GetItemsCommandHandler>().As<IGetItemsCommandHandler>().InstancePerDependency();

            builder.RegisterType<GetLodgingsAvailabilityCommandHandler>().As<IGetLodgingsAvailabilityCommandHandler>().InstancePerDependency();

            builder.RegisterType<GetLodgingsCommandHandler>().As<IGetLodgingsCommandHandler>().InstancePerDependency();

            builder.RegisterType<RebookingCommandHandler>().As<IRebookingCommandHandler>().InstancePerDependency();

            builder.RegisterType<UpdateBookingNoteCommandHandler>().As<IUpdateBookingNoteCommandHandler>().InstancePerDependency();

            builder.RegisterType<ValidateBookingCommandHandler>().As<IValidateBookingCommandHandler>().InstancePerDependency();

            builder.RegisterType<GetCustomerPrototypesCommandHandler>().As<IGetCustomerPrototypesCommandHandler>().InstancePerDependency();

            builder.RegisterType<AvailabilityConverter>().As<IAvailabilityConverter>().InstancePerDependency();

            builder.RegisterType<CompaniesConverter>().As<ICompaniesConverter>().InstancePerDependency();

            builder.RegisterType<ItemsConverter>().As<IItemsConverter>().InstancePerDependency();

            builder.RegisterType<BookingConverter>().As<IBookingConverter>().InstancePerDependency();
        }
    }
}