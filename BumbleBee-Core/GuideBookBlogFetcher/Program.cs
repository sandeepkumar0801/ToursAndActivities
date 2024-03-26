using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace GuideBookBlogFetcher
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var host = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder.UseStartup<Startup_Email>();

                   })
                   .Build();
            host.Run();
        }
    }
}


