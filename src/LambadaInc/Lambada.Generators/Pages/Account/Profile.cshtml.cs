using System.Threading.Tasks;
using Lambada.Generators.Infrastructure;
using Lambada.Generators.Options;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lambada.Generators.Pages.Account
{
    [Authorize]
    public class ProfilePageModel : GeneratorBasePageModel
    {
        private readonly IUserDataContext userDataContext;
        private readonly INotificationService notificationService;
        private readonly IAlertService alertService;
        private readonly ILogger<ProfilePageModel> logger;
        private readonly GeneratorOptions optionsValue;

        public ProfilePageModel(IUserDataContext userDataContext,
            INotificationService notificationService,
            IAlertService alertService,
            IOptions<GeneratorOptions> optionsValue,
                ILogger<ProfilePageModel> logger)
        {
            this.userDataContext = userDataContext;
            this.notificationService = notificationService;
            this.alertService = alertService;
            this.logger = logger;
            this.optionsValue = optionsValue.Value;
        }

        [BindProperty] public LambadaUser CurrentUser { get; set; }
        [BindProperty] public bool NotificationsEnabled { get; set; }

        public async Task OnGetAsync()
        {
            CurrentUser = userDataContext.GetCurrentUser();
            logger.LogInformation($"User {CurrentUser.FullName} loaded!");
            NotificationsEnabled = await alertService.GetInfoAboutAlertsAsync(CurrentUser.UserId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            logger.LogInformation($"Updating alerting for ");
            CurrentUser = userDataContext.GetCurrentUser();
            await alertService.EnableOrDisableAlertsAsync(NotificationsEnabled, CurrentUser.UserId);
            var infoText = $"Updated for {CurrentUser.FullName} to have alerts {(NotificationsEnabled ? "ON" : "OFF")}";
            InfoText = infoText;
            logger.LogInformation(infoText);

            if (NotificationsEnabled)
            {
                await notificationService.NotifyAsync(new EmailModel
                {
                    To = CurrentUser.Email,
                    From = optionsValue.DefaultEmailFrom, 
                    Content    = "You have been subscribed to notification. Disable it in profile anytime.",
                    Subject = $"{CurrentUser.FullName} your information has been updated"
                });
            }
            
            return RedirectToPage("/Account/Profile");
        }
    }
}