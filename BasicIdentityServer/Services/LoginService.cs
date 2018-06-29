using Identity.MongoDb;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BasicIdentityServer.Services
{
    public class LoginService : ILoginService<MongoIdentityUser>
    {
        private UserManager<MongoIdentityUser> _userManager;
        private SignInManager<MongoIdentityUser> _signInManager;

        public LoginService(UserManager<MongoIdentityUser> userManager, SignInManager<MongoIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<MongoIdentityUser> FindByUsername(string user)
        {
            return await _userManager.FindByEmailAsync(user).ConfigureAwait(false);
        }

        public async Task<bool> ValidateCredentials(MongoIdentityUser user, string password)
        {
            
            return await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false);
        }

        public Task SignIn(MongoIdentityUser user)
        {
          
            return _signInManager.SignInAsync(user, true);
        }
    }
}