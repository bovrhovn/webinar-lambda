using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Pages.Factories
{
    public class StatsPageModel : PageModel
    {
        private readonly INotificationService notificationService;
        private readonly IFactorySearchService factorySearchService;
        private readonly IStatsService statsService;
        private readonly ILogger<StatsPageModel> logger;

        public StatsPageModel(INotificationService notificationService,
            IFactorySearchService factorySearchService,
            IStatsService statsService,
            ILogger<StatsPageModel> logger)
        {
            this.notificationService = notificationService;
            this.factorySearchService = factorySearchService;
            this.statsService = statsService;
            this.logger = logger;
        }

        [BindProperty] public List<Factory> Factories { get; set; }
        [BindProperty] public List<FactoryStatModel> StatModels { get; set; }
        [BindProperty(SupportsGet = true)] public string FactoryId { get; set; }

        public async Task OnGetAsync()
        {
            //let's leverage Azure Sync option
            Factories = await factorySearchService.SearchFactoryAsync("");
            if (!string.IsNullOrEmpty(FactoryId)) 
                StatModels = await statsService.GetStatsForFactoryAsync(FactoryId);
        }
    }
}