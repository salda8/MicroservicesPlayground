using Identity.MongoDb;
using Identity.MongoDb.Models;
using System.ComponentModel.DataAnnotations;

namespace BasicIdentityServer.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public ApplicationUser User { get; set; }

        public MongoIdentityUser ToMongoIdentityUser()
        {
            var user = new MongoIdentityUser(Email, Email);
            return user;
    }
    }
}
