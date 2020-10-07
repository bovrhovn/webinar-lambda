using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lambada.Generators.Pages.Info
{
    public class AlertDetailsPageModel : PageModel
    {
        [BindProperty(SupportsGet = true)]public string ImageUrl { get; set; }
        public void OnGet(string alertUrl)
        {
            ImageUrl = alertUrl;
        }
    }
}