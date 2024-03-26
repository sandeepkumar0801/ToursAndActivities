//using Autofac.Extensions.DependencyInjection;
//using DataDumpingQueue.HangFire;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Hosting;
//using System.Net;

//namespace CacheLoader.HangFire
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var host = Host.CreateDefaultBuilder(args)
//                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
//                    .ConfigureWebHostDefaults(webBuilder =>
//                    {
//                        webBuilder.UseStartup<Startup_datadumpingQueue>();
//                        webBuilder.UseKestrel(opts =>
//                        {
//                            // Bind directly to a socket handle or Unix socket
//                            // opts.ListenHandle(123554);
//                            // opts.ListenUnixSocket("/tmp/kestrel-test.sock");
//                            //opts.Listen(IPAddress.Loopback, port: 5002);
//                            //opts.ListenAnyIP(5003);
//                            opts.ListenLocalhost(5004, opts => opts.UseHttps());
//                            opts.ListenLocalhost(5005, opts => opts.UseHttps());
//                        });

//                        webBuilder.UseKestrel()
//                            .UseUrls("http://localhost:5005")
//                            .UseStartup<Startup_cache>();
//                    })
//                    .Build();
//            host.Run();

//        }
//    }
//}