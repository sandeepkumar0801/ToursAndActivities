using Autofac;
using Isango.Persistence;
using Isango.Register;
using WebAPI;
using Microsoft.Extensions.Caching.Memory;
using Util;
using WebAPI.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Identity;
using WebAPI.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using log4net.Config;
using log4net;
using OfficeOpenXml;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System.Reflection;
using System.Xml;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;


    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {


        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                // Allow requests from any origin, method, and header.
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateActor = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };
        });
        services.AddAuthorization();
        services.AddEndpointsApiExplorer();
        services.AddHttpClient();
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Bearer Authentication with JWT Token",
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {    
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
 });

        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        services.AddMemoryCache();
       
        //services.AddScoped<ValidateModelAttribute>();
        services.AddControllers()
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.PropertyNamingPolicy = null; // Maintain the original property names
                 });
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        if (_env.IsProduction())
        {
            XmlConfigurator.Configure(new FileInfo("log4net.Production.config"));
        }
        else
        {
          XmlConfigurator.Configure(new FileInfo("log4net.config"));
        }
        services.AddSingleton(LogManager.GetLogger(typeof(Program)));

        // Resolve the memory cache instance from the DI container
        var memoryCache = services.BuildServiceProvider().GetService<IMemoryCache>();

        // Set the memory cache instance in the CacheHelper class
        CacheHelper.SetMemoryCache(memoryCache);
        RedixManagement.Initalize();
        services.AddHttpContextAccessor();

        services.AddHealthChecks();
        
        services.AddApplicationInsightsTelemetry(_configuration["APPINSIGHTS_CONNECTIONSTRING"]);
        

    }

    // ConfigureContainer is where you can register Autofac modules and other dependencies.
    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule<StartupModule>();

        builder.RegisterModule<APIModule>();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        if (env.IsProduction())
        {
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.Production.config"));

        }
        else
        {
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API");

            options.DocExpansion(DocExpansion.None);

            //Show the example by default
            options.DefaultModelRendering(ModelRendering.Example);


            options.EnableTryItOutByDefault();
            options.DocExpansion(DocExpansion.None);
            options.InjectStylesheet("/CSS/tokenStyle.css");
            options.InjectJavascript("/Scripts/CustomScript.js");
            //options.InjectJavascript("/Scripts/jquery-1.10.2.js");

           //options.InjectJavascript("/Scripts/tokenScript.js");

            //options.ConfigObject.AdditionalItems.Add("syntaxHighlight", false);



        });
       

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        

        //app.UseHttpContext();

        app.UseRouting();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseAuthentication();

        app.UseAuthorization();
        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseHealthChecks("/checkavailability");
    }

}
