using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.MongoDb;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace StsServerIdentity
{
    public class IdentityWithAdditionalClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<MongoIdentityUser> _claimsFactory;
        private readonly UserManager<MongoIdentityUser> _userManager;

        public IdentityWithAdditionalClaimsProfileService(UserManager<MongoIdentityUser> userManager, IUserClaimsPrincipalFactory<MongoIdentityUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string sub = context.Subject.GetSubjectId();

            MongoIdentityUser user = await _userManager.FindByIdAsync(sub);
            ClaimsPrincipal principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();

            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));

            //if (user.IsAdmin)
            //{
            //    claims.Add(new Claim(JwtClaimTypes.Role, "admin"));
            //}
            //else
            //{
            claims.Add(new Claim(JwtClaimTypes.Role, "user"));
            //}

            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string sub = context.Subject.GetSubjectId();
            MongoIdentityUser user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}