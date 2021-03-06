﻿using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GovUk.Education.ManageCourses.Ui
{
    public class Program
    {
        private const int DefaultPort = 44364;

        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo
                .ApplicationInsightsTraces(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"])
                .CreateLogger();

            BuildWebHost(args).Run();
        }

        public static int GetPort(string environment)
        {
            var envPort = Environment.GetEnvironmentVariable($"MANAGE_COURSES_API_{environment}_PORT");
            if (int.TryParse(envPort, out var portNumber) && portNumber >= 49152 && portNumber <= 65535)
            {
                return portNumber;
            }
            return DefaultPort;
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    if (environment == "Development")
                    {
                        var port = GetPort(environment);
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
                    }
                })
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .UseSentry(o =>
                {
                    o.MaxBreadcrumbs = 200;
                    o.MaxQueueItems = 100;
                    o.ShutdownTimeout = TimeSpan.FromSeconds(5);
                })
                .Build();
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
