using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Sales.Api.Configurations
{
    public static class SerilogConfigure
    {
        public static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile(Path.Combine("logs", @"log-{Date}.txt"))
                .WriteTo.MSSqlServer("SalesApi:DefaultConnection", "Logs", columnOptions: new ColumnOptions())
                .CreateLogger();
        }
    }
}
