using Identity.MongoDb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace BasicIdentityServer.Services
{
    public class LoginService : ILoginService<MongoIdentityUser>
    {
        private readonly IUserEmailStore<MongoIdentityUser> mongoUserStore;
        private UserManager<MongoIdentityUser> userManager;
        private SignInManager<MongoIdentityUser> signInManager;


        public LoginService(UserManager<MongoIdentityUser> userManager, SignInManager<MongoIdentityUser> signInManager, IUserEmailStore<MongoIdentityUser> mongoUserStore)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mongoUserStore = mongoUserStore;
        }

        public async Task<MongoIdentityUser> FindByUsername(string user)
        {
            return await userManager.FindByEmailAsync(user);
        }

        public async Task<bool> ValidateCredentials(MongoIdentityUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
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

        public Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
        {
            
            return signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        }

        public Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string authenticatorCode, bool rememberMe, bool rememberMachine)
        {
            return signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, rememberMachine);
        }

        public Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe, bool lockoutOnFailure)
        {
            return signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure);
        }

        public Task SignInAsync(MongoIdentityUser user, bool isPersistent)
        {
            return signInManager.SignInAsync(user, true);
        }

        public Task SignOutAsync()
        {
            return signInManager.SignOutAsync();
        }

        public Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool rememberMe, bool rememberBrowser)
        {
            return signInManager.TwoFactorSignInAsync(provider, code, rememberBrowser, rememberBrowser);
        }

        public async Task SetEmailAsConfirmed(MongoIdentityUser user)
        {
            await mongoUserStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);
        }

        public SignInManager<MongoIdentityUser> SignInManager => signInManager;
    }
}