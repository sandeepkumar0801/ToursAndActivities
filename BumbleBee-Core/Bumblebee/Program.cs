using Autofac;
using Autofac.Extensions.DependencyInjection;


var builder = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory()); // Use Autofac as the service provider factory

// Add services to the container.

var app = builder.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();
}).Build();

app.Run();
