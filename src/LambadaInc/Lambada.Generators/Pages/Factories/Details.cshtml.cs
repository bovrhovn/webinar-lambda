using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Pages.Factories
{
    public class DetailsPageModel : PageModel
    {
        private readonly IFactoryRepository factoryRepository;
        private readonly ILogger<DetailsPageModel> logger;

        public DetailsPageModel(IFactoryRepository factoryRepository, ILogger<DetailsPageModel> logger)
        {
            this.factoryRepository = factoryRepository;
            this.logger = logger;
        }

        [BindProperty] public Factory Factory { get; set; }
        [BindProperty, TempData] public string InfoText { get; set; }
        
        public async Task OnGetAsync(string factoryId)
        {
            logger.LogInformation($"Loading factory with ID {factoryId}");
            Factory = await factoryRepository.GetDataAsync(factoryId);
            var infoText = $"Factory {Factory.Name} loaded";
            InfoText = infoText;
            logger.LogInformation(infoText);
        }
    }
}