using System.IO.Compression;
using Lambada.Base;
using Lambada.Generators.Hubs;
using Lambada.Generators.Options;
using Lambada.Generators.Services;
using Lambada.Interfaces;
using Lambada.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lambada.Generators
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GeneratorOptions>(Configuration.GetSection("Generator"));
            services.Configure<SendGridOptions>(Configuration.GetSection("SendGridOptions"));
            services.Configure<StorageOptions>(Configuration.GetSection("StorageOptions"));
            services.Configure<IotOptions>(Configuration.GetSection("IotHub"));
            services.Configure<SearchServiceOptions>(Configuration.GetSection("SearchService"));

            var searchSettings = Configuration.GetSection("SearchService").Get<SearchServiceOptions>();
            services.AddScoped<IFactorySearchService, FactoryAzureSearchService>(_ =>
                new FactoryAzureSearchService(searchSettings.Name,
                    searchSettings.Key, searchSettings.FactoriesIndex));
            services.AddScoped<IFactorySearchResultService, FactoryAzureSearchResultService>(_ =>
                new FactoryAzureSearchResultService(searchSettings.Name,
                    searchSettings.Key, searchSettings.FactoryResultIndex));
            services.AddScoped<IUserDataContext, UserDataContext>();

            //email service configuration
            var sendGridSettings = Configuration.GetSection("SendGridOptions").Get<SendGridOptions>();
            services.AddScoped<IEmailService, SendGridEmailSender>(
                _ => new SendGridEmailSender(sendGridSettings.ApiKey));

            //repositories configuration
            var storageSettings = Configuration.GetSection("StorageOptions").Get<StorageOptions>();
            var userRepository = new UserRepository(storageSettings.ConnectionString, storageSettings.UsersTableName);
            services.AddTransient<IUserRepository, UserRepository>(_ => userRepository);

            var iotSettings = Configuration.GetSection("IotHub").Get<IotOptions>();
            var factoryDataService = new FactoryDataService(storageSettings.ConnectionString,
                storageSettings.FactoriesTableName, iotSettings.ConnectionString);
            services.AddTransient<IFactoryRepository, FactoryDataService>(_ => factoryDataService);

            var factoryDataResultService = new FactoryDeviceResultService(storageSettings.ConnectionString,
                storageSettings.FactoryResultTableName);
            services.AddTransient<IFactoryResultRepository, FactoryDeviceResultService>(_ => factoryDataResultService);

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddResponseCompression(options => options.Providers.Add<GzipCompressionProvider>());
            services.Configure<GzipCompressionProviderOptions>(compressionOptions =>
                compressionOptions.Level = CompressionLevel.Optimal);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddHttpContextAccessor();

            services.AddApplicationInsightsTelemetry();

            services.AddSignalR().AddAzureSignalR();

            services.AddRazorPages().AddRazorPagesOptions(options =>
                options.Conventions.AddPageRoute("/Info/Index", ""));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");

            app.UseStaticFiles();
            app.UseResponseCompression();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<MessagesHub>("/messages");
                endpoints.MapHub<AlertHub>("/alerts");
            });
        }
    }
}