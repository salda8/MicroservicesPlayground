using System.Threading;
using System.Threading.Tasks;
using Identity.MongoDb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace StsServerIdentity.Services
{
    public class LoginService : ILoginService<MongoIdentityUser>
    {
        private readonly IUserEmailStore<MongoIdentityUser> mongoUserStore;
        private SignInManager<MongoIdentityUser> signInManager;
        private UserManager<MongoIdentityUser> userManager;

        public LoginService(UserManager<MongoIdentityUser> userManager, SignInManager<MongoIdentityUser> signInManager, IUserEmailStore<MongoIdentityUser> mongoUserStore)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mongoUserStore = mongoUserStore;
        }

        public SignInManager<MongoIdentityUser> SignInManager => signInManager;

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl) => signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent) => signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);

        public Task<MongoIdentityUser> FindByUsername(string user) => userManager.FindByEmailAsync(user);

        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync() => signInManager.GetExternalLoginInfoAsync();

        public Task<MongoIdentityUser> GetTwoFactorAuthenticationUserAsync() => signInManager.GetTwoFactorAuthenticationUserAsync();

        public Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe, bool lockoutOnFailure) => signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure);

        public async Task SetEmailAsConfirmed(MongoIdentityUser user) => await mongoUserStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);

        public Task SignIn(MongoIdentityUser user) => signInManager.SignInAsync(user, true);

        public Task SignInAsync(MongoIdentityUser user, bool isPersistent) => signInManager.SignInAsync(user, true);

        public Task SignOutAsync() => signInManager.SignOutAsync();

        public Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string authenticatorCode, bool rememberMe, bool rememberMachine) => signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, rememberMachine);

        public Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode) => signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        public Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool rememberMe, bool rememberBrowser) => signInManager.TwoFactorSignInAsync(provider, code, rememberBrowser, rememberBrowser);

        public Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo info) => signInManager.UpdateExternalAuthenticationTokensAsync(info);

        public Task<bool> ValidateCredentials(MongoIdentityUser user, string password) => userManager.CheckPasswordAsync(user, password);
    }
}