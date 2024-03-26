using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataDumping.WebJob.Contracts;
using DataDumping.WebJob.Helper;
using Isango.Register;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging; // Add this using directive

namespace DataDumping.WebJob
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static TelemetryClient _telemetryClient;
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder();

            _telemetryClient = new TelemetryClient(new TelemetryConfiguration("28ac6a37-df01-4959-b2d2-de9634f91f7e"));

            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddTimers(); // Add this line for timer triggers
                b.AddExecutionContextBinding();
                b.AddAzureStorageQueues();
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                ConfigureAppConfiguration(config, context);
            });
            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();

                // If the key exists in settings, use it to enable Application Insights.
                string instrumentationKey = "28ac6a37-df01-4959-b2d2-de9634f91f7e";
                if (!string.IsNullOrEmpty(instrumentationKey))
                {
                    b.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = instrumentationKey);
                }
            });
            //builder.ConfigureServices((context, services) =>
            //{
            //    // Add logging
            //    services.AddLogging(logging =>
            //    {
            //        logging.ClearProviders(); // Clear any default providers
            //        logging.AddConsole(); // Add console logging
            //        // Add other logging providers if needed (e.g., Application Insights)
            //    });

            //    // ... other service registrations ...
            //});

            // Configure Autofac container and register services
            builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.ConfigureContainer<ContainerBuilder>(ConfigureContainer);

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
            // Start the host and run indefinitely
        }

        public static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<StartupModule>();
            builder.RegisterType<Functions>().InstancePerLifetimeScope();
            builder.RegisterType<DataDumpingHelper>().As<IDataDumpingHelper>().InstancePerLifetimeScope();
            

            // Additional registrations can be added here
        }

        private static void ConfigureAppConfiguration(IConfigurationBuilder config, HostBuilderContext context)
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                //Console.WriteLine("Environment: " + environment);
                if (environment != null)
                {

                    config.AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true);
                    // Additional configuration sources can be added here
                    _configuration = config.Build();
                }
                else
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    // Additional configuration sources can be added here
                    _configuration = config.Build();
                }

            }
            catch (Exception)
            {

            }
        }
    }
}
