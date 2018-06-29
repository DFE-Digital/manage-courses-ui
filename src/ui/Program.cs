using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace GovUk.Education.ManageCourses.Ui
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) {
            if (System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") 
            {
                return WebHost.CreateDefaultBuilder(args)
                    .UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.Listen(IPAddress.Loopback, 44364, listenOptions =>
                        {
                            listenOptions.UseHttps("dev-cert.pfx", "manage-courses");
                        });
                    })
                    .UseStartup<Startup>()
                    .Build();
            }
            else
            {
                return WebHost.CreateDefaultBuilder(args)
                    .UseKestrel(options => options.AddServerHeader = false)
                    .UseStartup<Startup>()
                    .Build();
            }
        }
    }
}
