using AsyncBooking.HangFire;
using Autofac;
using DiscountRuleEngine.Contracts;
using DiscountRuleEngine;
using Hangfire;
using Isango.Register;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using log4net.Config;
using log4net;
using System.Reflection;

namespace CacheLoader.HangFire
{
    public class Startup_cache
    {
        private readonly IConfiguration _configuration;

        public Startup_cache(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Load configuration based on the environment
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .Build();
            Console.WriteLine("Environmnet used for Hangfiree" +"-" + $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}json");

            services.AddSingleton<IConfiguration>(config);

            // Hangfire configuration
            var connectionString = config.GetConnectionString("HangFire");

            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(connectionString);
                config.UseSerializerSettings(new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });
                config.UseRecommendedSerializerSettings();
                config.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
            });
           // var memoryCache = services.BuildServiceProvider().GetService<IMemoryCache>();

            //CacheHelper.SetMemoryCache(memoryCache);

            // Add the processing server as IHostedService
            services.AddHangfireServer();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<StartupModule>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            app.UseHttpsRedirection();

            
            // Configure Hangfire dashboard options
            var appPath = _configuration["HangfireDashboardSettings:AppPath"];
            var options = new DashboardOptions()
            {
                AppPath = appPath,
                Authorization = new[] { new MyAuthorizationFilter() }

            };

            // Use Hangfire dashboard with the specified options
            app.UseHangfireDashboard("/isangodashboard1", options);

            // Schedule Hangfire jobs
            JobScheduler.JobScheduler_Hangfire();
        }

        public class MyAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context) => true;
        }
    }
}
