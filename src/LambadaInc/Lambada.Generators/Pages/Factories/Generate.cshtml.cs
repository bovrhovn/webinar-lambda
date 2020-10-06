using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lambada.Base;
using Lambada.Generators.Infrastructure;
using Lambada.Generators.Options;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Lambada.Generators.Pages.Factories
{
    [Authorize]
    public class GeneratePageModel : GeneratorBasePageModel
    {
        private readonly IFactoryRepository factoryRepository;
        private readonly ILogger<GeneratePageModel> logger;
        private readonly GeneratorOptions webOptions;
        private readonly RegistryManager registryManager;
        private readonly ServiceClient serviceClient;

        public GeneratePageModel(IFactoryRepository factoryRepository, ILogger<GeneratePageModel> logger,
            IOptions<IotOptions> optionsValue, IOptions<GeneratorOptions> webOptions)
        {
            this.factoryRepository = factoryRepository;
            this.logger = logger;
            this.webOptions = webOptions.Value;
            var valueConnectionString = optionsValue.Value.ConnectionString;
            registryManager = RegistryManager.CreateFromConnectionString(valueConnectionString);
            serviceClient = ServiceClient.CreateFromConnectionString(valueConnectionString);
        }

        [BindProperty] public Factory Factory { get; set; }
        [BindProperty] public int Number { get; set; }
        [BindProperty] public string Tags { get; set; }
        [BindProperty] public PaginatedList<FactoryDevice> Devices { get; set; }

        public async Task OnGetAsync(string factoryId, int? pageIndex)
        {
            logger.LogInformation($"Loading factory with ID {factoryId}");
            Factory = await factoryRepository.GetDataAsync(factoryId);
            var infoText = $"Factory {Factory.Name} loaded";
            InfoText = infoText;

            var query = registryManager.CreateQuery(
                $"SELECT * FROM devices WHERE tags.factory.id = '{factoryId}'", 100);
            var list = new List<FactoryDevice>();
            while (query.HasMoreResults)
            {
                var twins = await query.GetNextAsTwinAsync();
                foreach (var twin in twins)
                {
                    var tags = JsonConvert.DeserializeObject<Tags>(twin.Tags.ToJson());

                    var factory = tags.FactoryData;

                    list.Add(new FactoryDevice
                    {
                        FactoryId = factory.FactoryId,
                        Model = factory.Model,
                        DateCreated = DateTime.Parse(factory.Created),
                        FactoryDeviceId = twin.DeviceId
                    });
                }
            }

            logger.LogInformation($"Loaded {list.Count} items for factory {Factory.Name}");

            int page = pageIndex ?? 1;

            Devices = PaginatedList<FactoryDevice>.Create(list.AsQueryable(), page, webOptions.PageSize);

            logger.LogInformation(infoText);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int counter = 0;
            for (int currentItem = 0; currentItem < Number; currentItem++)
            {
                var deviceId = Guid.NewGuid().ToString();
                var device = new Device(deviceId);

                try
                {
                    await registryManager.AddDeviceAsync(device);
                    counter++;
                }
                catch (DeviceAlreadyExistsException ex)
                {
                    InfoText = $"Device is already added. Check ID.{Environment.NewLine}{ex.Message}";
                }

                var twin = await registryManager.GetTwinAsync(deviceId);

                var randomIndex = new Random().Next(0, Constants.DeviceModelNames.Length - 1);

                var deviceTagProperty = new DeviceTagProperty
                {
                    Tags = new Tags
                    {
                        FactoryData = new FactoryData
                        {
                            FactoryId = Factory.FactoryId,
                            Model = Constants.DeviceModelNames[randomIndex],
                            Created = DateTime.Now.ToShortDateString()
                        }
                    }
                };
                var patch = JsonConvert.SerializeObject(deviceTagProperty);
                await registryManager.UpdateTwinAsync(deviceId, patch, twin.ETag);
            }

            InfoText = $"Added {counter} devices with default tags...";

            //update device count
            var factory = await factoryRepository.GetDataAsync(Factory.FactoryId);
            factory.DeviceCount += 1;
            await factoryRepository.UpdateAsync(factory);

            //generate devices
            return RedirectToPage("/Factories/Details", new {factoryId = Factory.FactoryId});
        }
    }
}