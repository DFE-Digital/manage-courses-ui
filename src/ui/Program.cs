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
                int port = 44364; // in line with the default SSL port of IIS express                
                System.Int32.TryParse(System.Environment.GetEnvironmentVariable("PORT"), out port);

                return WebHost.CreateDefaultBuilder(args)
                    .UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.Listen(IPAddress.Loopback, port, listenOptions =>
                        {
                            // The following is a throwaway self-signed certifcate that has
                            // been added as a convenience to easily run this locally on HTTPs
                            // A cleaner way would be to generate an instance of
                            // X509Certificate2 on the fly but that requires a lot of faff
                            // ...and might even breakt 12 factor, see
                            // https://stackoverflow.com/questions/13806299/how-to-create-a-self-signed-certificate-using-c
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
