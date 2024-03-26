//using Autofac;
//using Hangfire;
//using Isango.Register;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DataDumpingQueue.HangFire
//{
//    public class Startup_datadumpingQueue
//    {
//        private readonly IConfiguration _configuration;

//        public Startup_datadumpingQueue(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public void ConfigureServices(IServiceCollection services)
//        {
//            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
//            //Console.WriteLine("Environment: " + environment);
//            if (environment != null)
//            {
//                IConfiguration config = new ConfigurationBuilder()
//            .AddJsonFile($"appsettings.{environment}.json", false, true)
//            .Build();
//             services.AddSingleton(config);
//            }
//            else
//            {
//                IConfiguration config = new ConfigurationBuilder()
//            .AddJsonFile($"appsettings.json", false, true)
//            .Build();
//             services.AddSingleton(config);
//            }

//            //Hangfire
//            var connectionString = _configuration.GetConnectionString("HangFire");
//            services.AddHangfire(config =>
//            {
//                config.UseSqlServerStorage(connectionString);
//                config.UseSerializerSettings(new JsonSerializerSettings
//                {
//                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
//                });
//                config.UseRecommendedSerializerSettings();
//                config.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
//            });

//            // Add the processing server as IHostedService
//            services.AddHangfireServer();


//        }

//        public void ConfigureContainer(ContainerBuilder builder)
//        {
//            builder.RegisterModule<StartupModule>();
//        }

//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            var appPath = _configuration["HangfireDashboardSettings:AppPath"];
//            var options = new DashboardOptions { AppPath = appPath };
//            app.UseHangfireDashboard("/isangodashboard", options);



//        }

//    }
//}
