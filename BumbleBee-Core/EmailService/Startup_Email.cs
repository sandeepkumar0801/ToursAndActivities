using Autofac;
using EmailSuitConsole.Data;
using EmailSuitConsole.Models;
using EmailSuitConsole.Service;
using FeefoDownloader;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Server;
using Isango.Register;
using log4net.Config;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Reflection;
using PreDepartMailer;
using System.Net;
using System.Text;

namespace EmailSuitConsole
{
    public class Startup_Email
    {
        private readonly IConfiguration _configuration;

        public Startup_Email(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
               .Build();
            Console.WriteLine("Environmnet used for Hangfire" + "-" + $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}json");

            services.AddSingleton<IConfiguration>(config);

            // Email services
            services.AddScoped<IEmailService, EmailService>();
            services.Configure<SMTPConfigModel>(config.GetSection("SMTPConfig"));
            services.Configure<EmailTemplateSettings>(config.GetSection("EmailTemplateSettings"));
            services.AddTransient<EmailConfig>();
            UserEmailOptions userEmailOptions = new UserEmailOptions();
            _configuration.GetSection("UserEmailOptions").Bind(userEmailOptions);
            services.AddSingleton(userEmailOptions);

            // Feefo services
            services.AddTransient<DataAccess>();
            services.AddTransient<Helper>();
            services.AddTransient<IsangoDataBaseLive>();
            services.AddTransient<DepartMethod>();
            
            services.AddTransient<Engine>();
            services.AddTransient<JobSchedulingService>();
            // Hangfire services
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        ValidateActor = true,
            //        ValidateAudience = true
            //    };
            //});
            services.AddAuthentication();
            var connectionString = config.GetConnectionString("HangFire");
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(connectionString);
                config.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
            });
            services.AddHangfireServer();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<StartupModule>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, JobSchedulingService jobSchedulingService)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            app.UseHttpsRedirection();
            //app.UseAuthentication();
            var appPath = _configuration["HangfireDashboardSettings:AppPath"];
            var options = new DashboardOptions()
            {
                AppPath = appPath,
                Authorization = new[] { new MyAuthorizationFilter() }
            };

            app.UseHangfireDashboard("/isangodashboard2", options);
            JobScheduler.JobScheduler_Hangfire();
        }

        public class MyAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context) => true;
        }
    }
}
