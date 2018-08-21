using BasicIdentityServer.Models;
using Identity.MongoDb;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BasicIdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<MongoIdentityUser> userManager;

        public ProfileService(UserManager<MongoIdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            Guard.ArgumentNotNull(nameof(context.Subject), context.Subject);
            
            var subject = context.Subject;

            var subjectId = subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject).Value;

            var user = await userManager.FindByIdAsync(subjectId);
            Guard.ArgumentNotNull(nameof(user), user);
          
            var claims = GetClaimsFromUser(user);
            context.IssuedClaims = claims.ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            Guard.ArgumentNotNull(nameof(context.Subject), context.Subject);
            var subject = context.Subject;

            var subjectId = subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
            var user = await userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type =="security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }
                bool isAccountActive = true;
                if (user.IsLockoutEnabled && user.LockoutEndDate != null)
                {
                    isAccountActive = user.LockoutEndDate.Instant >= DateTime.Now;
                }
                
                context.IsActive = isAccountActive;
            }
        }

        private IEnumerable<Claim> GetClaimsFromUser(MongoIdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject,user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            //if (!string.IsNullOrWhiteSpace(user.Name))
            //    claims.Add(new Claim("name", user.Name));

            //if (!string.IsNullOrWhiteSpace(user.LastName))
            //    claims.Add(new Claim("last_name", user.LastName));

            //if (!string.IsNullOrWhiteSpace(user.CardNumber))
            //    claims.Add(new Claim("card_number", user.CardNumber));

            //if (!string.IsNullOrWhiteSpace(user.CardHolderName))
            //    claims.Add(new Claim("card_holder", user.CardHolderName));

            //if (!string.IsNullOrWhiteSpace(user.SecurityNumber))
            //    claims.Add(new Claim("card_security_number", user.SecurityNumber));

            //if (!string.IsNullOrWhiteSpace(user.Expiration))
            //    claims.Add(new Claim("card_expiration", user.Expiration));

            //if (!string.IsNullOrWhiteSpace(user.City))
            //    claims.Add(new Claim("address_city", user.City));

            //if (!string.IsNullOrWhiteSpace(user.Country))
            //    claims.Add(new Claim("address_country", user.Country));

            //if (!string.IsNullOrWhiteSpace(user.State))
            //    claims.Add(new Claim("address_state", user.State));

            //if (!string.IsNullOrWhiteSpace(user.Street))
            //    claims.Add(new Claim("address_street", user.Street));

            //if (!string.IsNullOrWhiteSpace(user.ZipCode))
            //    claims.Add(new Claim("address_zip_code", user.ZipCode));

            if (userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.Email, user.NormalizedEmail),
                    new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            if (userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            return claims;
        }
    }
}
