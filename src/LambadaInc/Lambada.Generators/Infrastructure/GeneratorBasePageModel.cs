using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lambada.Generators.Infrastructure
{
    public class GeneratorBasePageModel : PageModel
    {
        [BindProperty, TempData] public string InfoText { get; set; }
    }
}