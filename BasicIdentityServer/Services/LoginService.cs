using Identity.MongoDb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BasicIdentityServer.Services
{
    public class LoginService : ILoginService<MongoIdentityUser>
    {
        private UserManager<MongoIdentityUser> userManager;
        private SignInManager<MongoIdentityUser> signInManager;

        public LoginService(UserManager<MongoIdentityUser> userManager, SignInManager<MongoIdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<MongoIdentityUser> FindByUsername(string user)
        {
            return await userManager.FindByEmailAsync(user).ConfigureAwait(false);
        }

        public async Task<bool> ValidateCredentials(MongoIdentityUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password).ConfigureAwait(false);
        }

        public Task SignIn(MongoIdentityUser user)
        {
            return signInManager.SignInAsync(user, true);
        }

        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            return signInManager.GetExternalLoginInfoAsync();
        }

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            return signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);
        }

        public Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo info)
        {
            return signInManager.UpdateExternalAuthenticationTokensAsync(info);
        }

        public Task<MongoIdentityUser> GetTwoFactorAuthenticationUserAsync()
        {
            return signInManager.GetTwoFactorAuthenticationUserAsync();
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
        {
            return signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }
    }
}