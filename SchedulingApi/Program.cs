using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Microsoft.AspNetCore.HealthChecks;

namespace SchedulingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                    .UseUrls("https://localhost:5002")
                    .UseHealthChecks("/health")
                    .UseStartup<Startup>()
                    .ConfigureServices(services => services.AddAutofac())
                    .UseSerilog((hostingContext, loggerConfiguration)
                                 => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
    }
}