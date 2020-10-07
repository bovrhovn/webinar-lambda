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

        [BindProperty] public Factory Factory { get; set; }
        [BindProperty(SupportsGet = true)] public int Hours { get; set; } = 0;
        public PaginatedList<SearchModel> RawDataResults { get; set; }
        
        public async Task OnGetAsync(string factoryId, int? pageIndex)
        {
            logger.LogInformation($"Loading factory with ID {factoryId}");
            Factory = await factoryRepository.GetDataAsync(factoryId);
            var infoText = $"Factory {Factory.Name} loaded";
            InfoText = infoText;
            logger.LogInformation(infoText);

            if (Hours != 0)
            {
                var list = await searchFactoryResultService.SearchByHoursAsync(Hours,options.PageSize);

                var message = $"Estimated time of getting back result - {list.Estimated}";
                logger.LogInformation(message);

                RawDataResults = PaginatedList<SearchModel>.Create(list.Items.AsQueryable(), pageIndex ?? 1,
                    options.PageSize);
                InfoText = infoText;
            }
        }
    }
}