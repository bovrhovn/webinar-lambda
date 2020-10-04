using System.ComponentModel.DataAnnotations;

namespace Lambada.Generators.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter email")]
        [Display(Name = "Email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter password")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}