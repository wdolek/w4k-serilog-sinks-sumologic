using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using W4k.Serilog.Sinks.SumoLogic.Extensions;

namespace W4k.Serilog.Sinks.SumoLogic.Sample
{
    public class Program
    {
        static void Main(string[] args)
        {
            // init logger programatically
            var url = new Uri("http://localhost");
            var sourceName = "w4k-serilog-sumologic";
            Logger logFromFluent = new LoggerConfiguration()
                .WriteTo.SumoLogic(url, sourceName: sourceName)
                .CreateLogger();

            logFromFluent.Information("Event: Initialized logger");

            // init logger from configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Logger logFromConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            logFromConfig.Information("Event: Configured logger");

            // we are done!
            Console.ReadLine();
        }
    }
}
