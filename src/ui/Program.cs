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

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => options.AddServerHeader = false)
                .UseStartup<Startup>()
                .Build();
    }
}
