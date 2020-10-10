using System;
using System.Threading.Tasks;
using Lambada.Generators.Helpers;
using Lambada.Generators.Infrastructure;
using Lambada.Generators.Options;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lambada.Generators.Pages.Account
{
    [AllowAnonymous]
    public class RegisterPageModel : GeneratorBasePageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<RegisterPageModel> logger;
        private readonly INotificationService notificationService;
        private readonly GeneratorOptions generatorOptions;

        public RegisterPageModel(IUserRepository userRepository,
            ILogger<RegisterPageModel> logger,
            IOptions<GeneratorOptions> generateOptionsValue,
            INotificationService notificationService)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            generatorOptions = generateOptionsValue.Value;
            this.notificationService = notificationService;
        }

        [BindProperty] public LambadaUser NewUser { get; set; }

        public void OnGet() => logger.LogInformation("Loading registration form");

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

            var emailModel = new EmailModel
            {
                From = generatorOptions.DefaultEmailFrom,
                To = currentUser.Email,
                Content = "You have account activated and can use the site.",
                Subject = $"{currentUser.FullName}, your account was created"
            };

            await notificationService.NotifyAsync(emailModel);

            return RedirectToPage("/Info/Index");
        }
    }
}