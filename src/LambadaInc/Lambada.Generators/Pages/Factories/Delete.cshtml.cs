using System;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Pages.Factories
{
    public class DeletePageModel : PageModel
    {
        private readonly IFactoryRepository factoryRepository;
        private readonly ILogger<DeletePageModel> logger;

        public DeletePageModel(IFactoryRepository factoryRepository, ILogger<DeletePageModel> logger)
        {
            this.factoryRepository = factoryRepository;
            this.logger = logger;
        }
        
        [BindProperty] public Factory Factory { get; set; }
        [BindProperty, TempData] public string InfoText { get; set; }
        
        public async Task OnGetAsync(string factoryId)
        {
            logger.LogInformation($"Loading factory with {factoryId}");
            Factory = await factoryRepository.GetDataAsync(factoryId);
            var infoText = $"Factory {Factory.Name} loaded";
            InfoText = infoText;
            logger.LogInformation(infoText);
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            logger.LogInformation($"Deleting {Factory.FactoryId} - {Factory.Name}");
            await factoryRepository.DeleteAsync(Factory.FactoryId);
            var infoText = $"Factory {Factory.Name} deleted at {DateTime.Now}";
            InfoText = infoText;
            logger.LogInformation(infoText);
            return RedirectToPage("/Factories/Overview");
        }
    }
}