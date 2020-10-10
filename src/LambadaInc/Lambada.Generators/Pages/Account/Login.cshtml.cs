using System;
using System.Threading.Tasks;
using Lambada.Generators.Helpers;
using Lambada.Generators.Infrastructure;
using Lambada.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Pages.Account
{
    [AllowAnonymous]
    public class LoginPageModel : GeneratorBasePageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<LoginPageModel> logger;

        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public string ReturnUrl { get; set; }
        
        public LoginPageModel(IUserRepository userRepository, ILogger<LoginPageModel> logger)
        {
            this.userRepository = userRepository;
            this.logger = logger;
        }

        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl;
            logger.LogInformation($"Return url {returnUrl} has been set...");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            logger.LogInformation("Logging in user {Email}");
            var currentUser = await userRepository.LoginAsync(Email, Password);
            if (currentUser == null) return RedirectToPage("Login");

            logger.LogInformation($"User {Email} logged in at {DateTime.Now}");
            
            await HttpContext.SignInAsync(currentUser.GenerateClaims());

            if (!string.IsNullOrEmpty(ReturnUrl))
                return Redirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : "/");
            
            logger.LogInformation("Redirecting to default route - index");
            return RedirectToPage("/Info/Index");
        }
    }
}