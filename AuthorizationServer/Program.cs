using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace AuthorizationServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Authorization Server";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile(@"logs\log-{Date}.txt")
                .CreateLogger();
            try
            {
                Log.Information("Starting Authorization Server host");
                
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseUrls(Environment.GetEnvironmentVariable("AuthorizationServer:ServerBase"))
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
