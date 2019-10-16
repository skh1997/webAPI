using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sales.Api.Configurations;
using Sales.Infrastructure.Data;
using Serilog;

namespace Sales.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Sales API";
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var salesContext = services.GetRequiredService<SalesContext>();
                    SalesContextSeed.SeedAsync(salesContext, loggerFactory).Wait();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseUrls(Environment.GetEnvironmentVariable("SalesApi:ServerBase"))
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
