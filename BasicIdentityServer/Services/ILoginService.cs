using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BasicIdentityServer.Services
{
    public interface ILoginService<T>
    {
        Task<bool> ValidateCredentials(T user, string password);
        Task<T> FindByUsername(string user);
        Task SignIn(T user);
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);
        Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo info);
        Task<T> GetTwoFactorAuthenticationUserAsync();
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
    }
}
