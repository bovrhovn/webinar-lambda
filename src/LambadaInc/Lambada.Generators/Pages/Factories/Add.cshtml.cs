using System;
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
    public class AddPageModel : GeneratorBasePageModel
    {
        private readonly IFactoryRepository factoryRepository;
        private readonly ILogger<AddPageModel> logger;
        [BindProperty] public Factory Factory { get; set; } = new Factory();

        public AddPageModel(IFactoryRepository factoryRepository, ILogger<AddPageModel> logger)
        {
            this.factoryRepository = factoryRepository;
            this.logger = logger;
        }

        public void OnGet() => InfoText = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            logger.LogInformation("Adding factory...");
            await factoryRepository.AddAsync(Factory);
            InfoText = $"Factory {Factory.Name} added at {DateTime.Now}";
            logger.LogInformation(InfoText);
            return RedirectToPage("/Factories/Overview");
        }
    }
}