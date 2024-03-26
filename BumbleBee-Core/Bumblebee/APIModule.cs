using Autofac;
using WebAPI.Helper;
using WebAPI.Mapper;

namespace WebAPI
{
    /// <summary>
    /// Register all modules required for API
    /// </summary>
    public class APIModule : Module
    {
        /// <summary>
        /// Load all modules required for API.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActivityMapper>().InstancePerLifetimeScope();
            builder.RegisterType<BookingMapper>().InstancePerLifetimeScope();
            builder.RegisterType<DiscountMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SubscribeMapper>().InstancePerLifetimeScope();
            builder.RegisterType<ActivityHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutMapper>().InstancePerLifetimeScope();
            builder.RegisterType<MasterMapper>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleMapsMapper>().InstancePerLifetimeScope();
            builder.RegisterType<BookingHelper>().InstancePerLifetimeScope();
          builder.RegisterType<GoogleMapsHelper>().InstancePerLifetimeScope();
          builder.RegisterType<CancellationMapper>().InstancePerLifetimeScope();
          builder.RegisterType<CancellationHelper>().InstancePerLifetimeScope();
          builder.RegisterType<MasterHelper>().InstancePerLifetimeScope();
        }
    }
}