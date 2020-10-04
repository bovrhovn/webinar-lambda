using System;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lambada.Generators.Pages.Factories
{
    public class AddPageModel : PageModel
    {
        private readonly IFactoryRepository factoryRepository;
        [BindProperty, TempData] public string InfoText { get; set; }
        [BindProperty] public Factory Factory { get; set; } = new Factory();

        public AddPageModel(IFactoryRepository factoryRepository)
        {
            this.factoryRepository = factoryRepository;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await factoryRepository.AddAsync(Factory);
            InfoText = $"Factory {Factory.Name} added at {DateTime.Now}";
            return RedirectToPage("/Factories/Overview");
        }
    }
}