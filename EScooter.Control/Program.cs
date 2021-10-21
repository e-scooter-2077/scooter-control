using EasyDesk.CleanArchitecture.Infrastructure.DependencyInjection;
using EScooter.Control.Application;
using EScooter.Control.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace EScooter.Control
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    var dict = new Dictionary<string, string>();
                    dict.Add("IotHubConfiguration:ConnectionString", Environment.GetEnvironmentVariable("IotHubConfiguration__ConnectionString"));
                    dict.Add("IotHubConfiguration:HostName", Environment.GetEnvironmentVariable("IotHubConfiguration__ConnectionString"));
                    configurationBuilder.AddInMemoryCollection(dict);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddConfigAsSingleton<IotHubConfiguration>(hostContext.Configuration);
                    services.AddSingleton<IIotHubRegistryManager, IotHubRegistryManager>();
                    services.AddSingleton<LockCommandsHandler>();
                    services.AddSingleton<TelemetryHandler>();
                });
    }
}
