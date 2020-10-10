using Lambada.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Pages.Factories
{
    public class StatsPageModel : PageModel
    {
        private readonly INotificationService notificationService;
        private readonly ILogger<StatsPageModel> logger;

        public StatsPageModel(INotificationService notificationService, ILogger<StatsPageModel> logger)
        {
            this.notificationService = notificationService;
            this.logger = logger;
        }

        public void OnGet()
        {
            
        }
    }
}