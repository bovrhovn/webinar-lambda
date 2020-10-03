using System.Linq;
using System.Threading.Tasks;
using Lambada.Base;
using Lambada.Generators.Interfaces;
using Lambada.Generators.Options;
using Lambada.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lambada.Generators.Pages.Factories
{
    public class OverviewPageModel : PageModel
    {
        private readonly IFactorySearchService searchService;
        private readonly ILogger<OverviewPageModel> logger;
        private readonly GeneratorOptions options;
        [BindProperty, TempData] public string InfoText { get; set; }
        [BindProperty(SupportsGet = true)] public string Query { get; set; }

        public OverviewPageModel(IFactorySearchService searchService,
            IOptions<GeneratorOptions> options,
            ILogger<OverviewPageModel> logger)
        {
            this.searchService = searchService;
            this.logger = logger;
            this.options = options.Value;
        }

        public PaginatedList<SearchModel> Factories { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            var list = await searchService.SearchAsync(Query);
            
            logger.LogInformation("Estimated time of getting back result - " + list.Estimated);
            
            Factories = PaginatedList<SearchModel>.Create(list.Items.AsQueryable(), pageIndex ?? 1, options.PageSize);

            var infoText = $"{list.Items.Count} factories loaded!";
            
            InfoText = infoText;
            logger.LogInformation(infoText);
        }
    }
}