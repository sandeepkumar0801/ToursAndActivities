using Autofac;
using Microsoft.Extensions.Configuration;
using DataDumping.WebJob.Helper;
using DataDumping.WebJob.Contracts;
using Isango.Service.ConsoleApplication;
using Isango.Service.Contract;

namespace DataDumping.WebJob.Register
{
    public class WebJobModule : Module
    {
        private readonly IConfiguration _configuration;

        public WebJobModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Register your services and dependencies here.
            builder.RegisterType<ServiceAvailabilityService>().As<IServiceAvailabilityService>().SingleInstance();
            builder.RegisterType<Functions>();

            builder.RegisterType<DataDumpingHelper>().As<IDataDumpingHelper>().InstancePerLifetimeScope();

        }
    }
}
