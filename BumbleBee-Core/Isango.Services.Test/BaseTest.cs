using Autofac;
using Isango.Register;
using Microsoft.Extensions.Configuration;

namespace Isango.Services.Test
{
    public class BaseTest
    {
        protected IContainer _container;
        protected IConfigurationRoot _configuration;

        public BaseTest()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine("Environment: " + environment);
            if (environment != null)
            {
                _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to your test project directory
                .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                .Build();


            }
            else
            {
                _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to your test project directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            }

            var builder = new ContainerBuilder();
            builder.RegisterInstance(_configuration).As<IConfiguration>();
            builder.RegisterModule<StartupModule>();
            _container = builder.Build();
        }
    }
}