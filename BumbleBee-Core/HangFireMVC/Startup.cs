using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using HangFireMVC.Helper;
using HangFireMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HangFireMVC
{
    public class Startup
    {

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                           options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.SignIn.RequireConfirmedEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });
            services.AddControllersWithViews();
            services.AddRazorPages();
            
            services.AddScoped<IAccountRepository, AccountRepository>();

            //Hangfire
            var connectionString = _configuration.GetConnectionString("HangFireMain");
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(connectionString);
                config.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
            });

            // Add the processing server as IHostedService
            services.AddHangfireServer();



        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseHangfireServer();

            var connectionString1 = _configuration.GetConnectionString("HangFire");
            var storage1 = new SqlServerStorage(connectionString1);
            var options1 = new DashboardOptions
            {
                AppPath = "/Home/Index",
                DashboardTitle = "Isango DashBoard",
                Authorization = new[] { new DashboardAuthorizationFilter() }
            };
            app.UseHangfireDashboard("/isangodashboard", options1, storage1);

            //var connectionString2 = _configuration.GetConnectionString("Hangfire2");
            //var storage2 = new SqlServerStorage(connectionString2);
            //var options2 = new DashboardOptions
            //{
            //    AppPath = "/Home/Index",
            //    DashboardTitle = "Isango server 2",
            //    Authorization = new[] { new DashboardAuthorizationFilter() }
            //};
            //app.UseHangfireDashboard("/isangodashboard2", options2, storage2);

            /*
            app.UseHangfireDashboard("/isangodashboard", new DashboardOptions
            {
                AppPath = "/Account/SignOut",
                DashboardTitle = "Isango DashBoard",
                Authorization = new[] { new DashboardAuthorizationFilter() }

            }) ; */

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
                endpoints.MapControllerRoute(
                   name: "jobs",
                   pattern: "{controller=Job}/{action=ScheduleJobs}");
                endpoints.MapRazorPages();

            });
        }
    }
}
