
using Identity.MongoDb;
using Identity.MongoDb.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BasicIdentityServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser {

                
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