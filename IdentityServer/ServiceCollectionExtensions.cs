using System;
using Identity.MongoDb;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StsServerIdentity.Services;
using StsServerIdentity.Settings;

namespace StsServerIdentity
{
    public static class ServiceCollectionExtensions
    {
        public static IdentityBuilder AddIdentity<TUser>(this IServiceCollection services)
            where TUser : class => services.AddIdentity<TUser>(null);

        public static IdentityBuilder AddIdentity<TUser>(this IServiceCollection services, Action<IdentityOptions> setupAction)
            where TUser : class
        {
            // Services used by identity
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.LoginPath = new PathString("/Account/Login");
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                };
            })
            .AddCookie(IdentityConstants.ExternalScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.ExternalScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme,
                o => o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme)
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });

            // Hosting doesn't add IHttpContextAccessor by default
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Identity services
            services.TryAddScoped<IUserValidator<TUser>, UserValidator<TUser>>();
            services.TryAddScoped<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
            services.TryAddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();

            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<TUser>>();
            //services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, ClaimsPrincipalFactory>();
            services.TryAddScoped<UserManager<TUser>, AspNetUserManager<TUser>>();
            services.TryAddScoped<SignInManager<TUser>, SignInManager<TUser>>();
            //services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser>>();

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return new IdentityBuilder(typeof(TUser), services);
        }

        internal static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IRedirectService, RedirectService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<ILoginService<MongoIdentityUser>, LoginService>();
        }

        internal static void ConfigureOptions(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<EmailSenderOptions>(Configuration.GetSection("EmailService"));
            services.Configure<SmsSenderOptions>(Configuration.GetSection("SmsService"));

            services.Configure<FacebookApiOptions>(Configuration.GetSection("FacebookApi"));

            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));

            services.Configure<GoogleApiOptions>(Configuration.GetSection("GoogleApi"));
        }
    }
}