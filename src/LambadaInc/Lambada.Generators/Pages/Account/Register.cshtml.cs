using System;
using System.Threading.Tasks;
using Lambada.Generators.Helpers;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Pages.Account
{
    [AllowAnonymous]
    public class RegisterPageModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<RegisterPageModel> logger;

        public RegisterPageModel(IUserRepository userRepository, ILogger<RegisterPageModel> logger)
        {
            this.userRepository = userRepository;
            this.logger = logger;
        }

        [BindProperty] public LambadaUser NewUser { get; set; }
        [BindProperty, TempData] public string InfoText { get; set; }
        
        public void OnGet()
        {
            logger.LogInformation("Loading registration form");
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            logger.LogInformation($"Registering user at {DateTime.Now}");
            var currentUser = await userRepository.RegisterAsync(NewUser);
            if (currentUser == null)
            {
                InfoText = "Registering was not successful. Try again later!";
                return RedirectToPage("Register");
            }

            logger.LogInformation($"Logged in at {DateTime.Now}");
            await HttpContext.SignInAsync(currentUser.GenerateClaims());

            return RedirectToPage("/Info/Index");
        }
    }
}