﻿using System;
using Serilog;
using Serilog.Context;
using Serilog.Formatting.Json;
using Serilog.Sinks.Humio;

namespace SerilogAndHumio {
    internal class Program {
        static void Main(string[] args)
        {
            // Set JsonFormatter for Humio sink
            var humioConfig = new HumioSinkConfiguration
            {
                IngestToken = "{humioToken}",
                TextFormatter = new JsonFormatter(renderMessage: true),
                Url = "https://cloud.community.humio.com"
            };

            // Serilog configuration
            using var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()          
                .Enrich.WithProperty("FrameworkVersion", Environment.Version)
                .WriteTo.Console()
                .WriteTo.HumioSink(humioConfig)
                .CreateLogger();

            // Set default Serilog logger to the one 
            // obtained by LoggerConfiguration
            Log.Logger = log;

            // Set context information that will be available in the 
            // formatted message
            using (LogContext.PushProperty("args", args))
            using (LogContext.PushProperty("assembly", typeof(Program).Assembly.FullName))
            using (LogContext.PushProperty("class", typeof(Program).FullName))

                Log.Information("This is a message from a test Console Application that use Serilog with Humio");

            Console.ReadLine();
        }
    }
}
