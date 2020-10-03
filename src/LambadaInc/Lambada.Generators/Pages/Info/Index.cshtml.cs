using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Pages.Info
{
    public class IndexPageModel : PageModel
    {
        private ILogger<IndexPageModel> logger;

        public IndexPageModel(ILogger<IndexPageModel> logger)
        {
            this.logger = logger;
        }

        public void OnGet()
        {
            logger.LogInformation("Page Info successfuly loaded.");
        }
    }
}