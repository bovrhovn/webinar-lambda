using System;
using Lambada.Interfaces;
using Lambada.Services;
using LambadaInc.Generators;
using LambadaInc.Generators.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace LambadaInc.Generators
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IFactoryResultRepository, FactoryDeviceResultService>(_=>
                new FactoryDeviceResultService(Environment.GetEnvironmentVariable("GenerateOptions:StorageKey"),
                    Environment.GetEnvironmentVariable("GenerateOptions:ResultTableName")));
            builder.Services.AddScoped<IFactoryRepository, FactoryDataService>(_=>
                new FactoryDataService(Environment.GetEnvironmentVariable("GenerateOptions:StorageKey"),
                    Environment.GetEnvironmentVariable("GenerateOptions:FactoryTableName"),
                    Environment.GetEnvironmentVariable("GenerateOptions:DeviceConnectionString")));
            builder.Services.AddOptions<GenerateOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.GetSection("GenerateOptions").Bind(settings));
        }
    }
}