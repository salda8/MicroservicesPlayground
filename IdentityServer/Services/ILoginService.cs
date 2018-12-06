using System.Threading.Tasks;
using Identity.MongoDb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace StsServerIdentity.Services
{
    public interface ILoginService<T>
    {
        SignInManager<MongoIdentityUser> SignInManager { get; }

        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);

        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);

        Task<T> FindByUsername(string user);

        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();

        Task<T> GetTwoFactorAuthenticationUserAsync();

        Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe, bool lockoutOnFailure);

        Task SetEmailAsConfirmed(MongoIdentityUser user);

        Task SignIn(T user);

        Task SignInAsync(MongoIdentityUser user, bool isPersistent);

        Task SignOutAsync();

        Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string authenticatorCode, bool rememberMe, bool rememberMachine);

        Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);

        Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool rememberMe, bool rememberBrowser);

        Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo info);

        Task<bool> ValidateCredentials(T user, string password);
    }
}