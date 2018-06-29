using System.ComponentModel.DataAnnotations;

namespace BasicIdentityServer.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
