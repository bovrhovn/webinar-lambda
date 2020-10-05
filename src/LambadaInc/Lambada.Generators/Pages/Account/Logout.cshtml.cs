using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Pages.Account
{
    public class LogoutPageModel : PageModel
    {
        private readonly ILogger<LogoutPageModel> logger;

        public LogoutPageModel(ILogger<LogoutPageModel> logger) => this.logger = logger;

        public async Task<RedirectToPageResult> OnGet()
        {
            logger.LogInformation("Logging current user out...");
            await HttpContext.SignOutAsync();
            logger.LogInformation("logout successful, doing redirect...");
            return RedirectToPage("/Info/Index");
        }
    }
}