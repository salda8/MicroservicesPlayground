using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Extensions.Logging;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);
            builder.ConfigureServices(s => s.AddSingleton(builder))
            .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                          .AddJsonFile("appsettings.json")
                          .AddJsonFile("ocelot2.json")
                          .AddEnvironmentVariables();
                })
                .ConfigureLogging((context, b) =>
                {
                    b.AddConfiguration(context.Configuration.GetSection("Logging"));
                    b.AddDynamicConsole();
                })
                .UseStartup<Startup>();
            IWebHost host = builder.Build();
            return host;
        }
    }
}
