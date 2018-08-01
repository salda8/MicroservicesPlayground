using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Ordering.SignalrHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("https://localhost:5004")
                .UseStartup<Startup>()
                .Build();
    }
}
