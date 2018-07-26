using BasicIdentityServer.Services;
using Identity.MongoDb;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace BasicIdentityServer
{
    public class ResourceOwnedPasswordValidation : IResourceOwnerPasswordValidator
    {
        private readonly ILoginService<MongoIdentityUser> loginService;

        public ResourceOwnedPasswordValidation(ILoginService<MongoIdentityUser> loginService)
        {
            this.loginService = loginService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            MongoIdentityUser user = await loginService.FindByUsername(context.UserName);

            if (await loginService.ValidateCredentials(user, context.Password))
            {
                
                context.Result = new GrantValidationResult(user.Id, OidcConstants.AuthenticationMethods.Password);
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,"invalid credential");
            }
        }
    }
}