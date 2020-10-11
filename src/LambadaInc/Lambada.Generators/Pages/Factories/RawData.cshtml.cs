using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lambada.Base;
using Lambada.Generators.Infrastructure;
using Lambada.Generators.Options;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Lambada.Generators.Pages.Factories
{
    public class RawDataPageModel : GeneratorBasePageModel
    {
        private readonly IFactoryRepository factoryRepository;
        private readonly IFactoryResultRepository factoryResultRepository;
        private readonly ISearchFactoryResultService searchFactoryResultService;
        private readonly ILogger<DetailsPageModel> logger;
        private readonly GeneratorOptions options;

        public RawDataPageModel(IFactoryRepository factoryRepository,
            IFactoryResultRepository factoryResultRepository,
            IOptions<GeneratorOptions> options,
            ISearchFactoryResultService searchFactoryResultService,
            ILogger<DetailsPageModel> logger)
        {
            this.factoryRepository = factoryRepository;
            this.factoryResultRepository = factoryResultRepository;
            this.searchFactoryResultService = searchFactoryResultService;
            this.logger = logger;
            this.options = options.Value;
        }

        [BindProperty(SupportsGet = true)] public int Hours { get; set; }

        public PaginatedList<SearchModel> RawDataResults { get; set; } =
            new PaginatedList<SearchModel>(new List<SearchModel>(), 0, 0, 50);

        public async Task OnGetAsync(int? pageIndex)
        {
            logger.LogInformation($"Loading data...");
            var infoText = "Loading results for device data";
            
            var list = await searchFactoryResultService.SearchByHoursAsync(Hours);

            var message = $"Estimated item count of getting back result - {list.ItemCount}";
            logger.LogInformation(message);

            RawDataResults = PaginatedList<SearchModel>.Create(list.Items.AsQueryable(),
                pageIndex ?? 1,
                (int) list.ItemCount);
            InfoText = infoText;
        }
    }
}