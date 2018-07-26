using Identity.MongoDb;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BasicIdentityServer
{
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<MongoIdentityUser, MongoIdentityRole>
    {
        public ClaimsPrincipalFactory(UserManager<MongoIdentityUser> userManager, RoleManager<MongoIdentityRole> roleManager, IOptions<IdentityOptions> optionsAccessor)
                    : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(MongoIdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (!identity.HasClaim(x => x.Type == JwtClaimTypes.Subject))
            {
                identity.AddClaim(new Claim(JwtClaimTypes.Subject, user.Id));
            }

            return identity;
        }
    }
}