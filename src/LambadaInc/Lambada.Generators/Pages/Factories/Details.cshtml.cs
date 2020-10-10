using System.Threading.Tasks;
using Lambada.Generators.Infrastructure;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Pages.Factories
{
    [Authorize]
    public class DetailsPageModel : GeneratorBasePageModel
    {
        private readonly IFactoryRepository factoryRepository;
        private readonly ILogger<DetailsPageModel> logger;

        public DetailsPageModel(IFactoryRepository factoryRepository, ILogger<DetailsPageModel> logger)
        {
            this.factoryRepository = factoryRepository;
            this.logger = logger;
        }

        [BindProperty] public Factory Factory { get; set; }

        public async Task OnGetAsync(string factoryId)
        {
            logger.LogInformation($"Loading factory with ID {factoryId}");
            Factory = await factoryRepository.GetDataAsync(factoryId);

            if (Factory == null)
            {
                InfoText = "There has been an error getting back the data from database.";
            }
            else
            {
                var infoText = $"Factory {Factory.Name} loaded";
                InfoText = infoText;
                logger.LogInformation(infoText);
            }
        }
    }
}