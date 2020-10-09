using System;
using Lambada.Base;
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
            builder.Services.AddScoped<IFactoryResultRepository, FactoryDeviceResultService>(_ =>
                new FactoryDeviceResultService(Environment.GetEnvironmentVariable("GenerateOptions:StorageKey"),
                    Environment.GetEnvironmentVariable("GenerateOptions:ResultTableName")));

            builder.Services.AddScoped<IStorageWorker, AzureStorageWorker>(_ =>
                new AzureStorageWorker(Environment.GetEnvironmentVariable("GenerateOptions:StorageKey"),
                    Environment.GetEnvironmentVariable("StorageOptions:ContainerName")));

            var factoryAzureSearch = new FactoryAzureSearchService(
                Environment.GetEnvironmentVariable("GenerateOptions:AzureSearchName"),
                Environment.GetEnvironmentVariable("GenerateOptions:AzureSearchKey"),
                Environment.GetEnvironmentVariable("GenerateOptions:AzureSearchFactoriesIndex")
            );

            var cosmoConn = Environment.GetEnvironmentVariable("GenerateOptions:CosmosDbConnectionString");
            var cosmosDbName = Environment.GetEnvironmentVariable("GenerateOptions:CosmosDbDatabase");

            builder.Services.AddScoped<IAlertService, CosmosDbAlertService>(_ =>
                new CosmosDbAlertService(
                    cosmoConn,
                    cosmosDbName,
                    Environment.GetEnvironmentVariable("GenerateOptions:AlertCollection")));

            builder.Services.AddScoped<IFactoryRepository, FactoryDataServiceCosmoDb>(_ =>
                new FactoryDataServiceCosmoDb(
                    cosmoConn,
                    cosmosDbName,
                    Environment.GetEnvironmentVariable("GenerateOptions:FactoryContainerName"),
                    Environment.GetEnvironmentVariable("GenerateOptions:DeviceConnectionString"),
                    factoryAzureSearch));
            builder.Services.AddOptions<GenerateOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.GetSection("GenerateOptions").Bind(settings));
        }
    }
}