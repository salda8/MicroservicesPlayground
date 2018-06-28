
using Identity.MongoDb;
using Identity.MongoDb.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : MongoIdentityUser
    {
        public ApplicationUser(string userName, string email) : base(userName, email)
        {
        }

        public ApplicationUser(string userName, MongoUserEmail email) : base(userName, email)
        {
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string SecurityNumber { get; set; }

        [Required]
        [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
        public string Expiration { get; set; }

        [Required]
        public string CardHolderName { get; set; }

        public int CardType { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }
    }

    //public class ApplicationUserClaimsFactory : IUserClaimsPrincipalFactory<ApplicationUser>
    //{
    //    private readonly UserManager<ApplicationUser> userManager;

    //    public ApplicationUserClaimsFactory(UserManager<ApplicationUser> userManager)
    //    {
    //        this.userManager = userManager;
    //    }

    //    public Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    //    {
            
    //        return  userManager.AddClaimAsync(user,new Claim(ClaimTypes.Role, "user"));
    //    }
    //}
}