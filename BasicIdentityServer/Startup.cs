using BasicIdentityServer.Configuration;
using BasicIdentityServer.Services;
using Identity.MongoDb;
using IdentityServer4;
using IdentityServer4.MongoDB.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace BasicIdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddIdentity<MongoIdentityUser, MongoIdentityRole>()
                    .AddClaimsPrincipalFactory<ClaimsPrincipalFactory>()
                    .AddDefaultTokenProviders()
                    .AddRoles<MongoIdentityRole>()
                    .AddUserStore<MongoUserStore<MongoIdentityUser>>()
                    .AddRoleStore<MongoRoleClaimStore<MongoIdentityRole>>();

            services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
            })
                .AddConfigurationStore(Configuration.GetSection("MongoDb"))
                .AddOperationalStore(Configuration.GetSection("MongoDb"))
                .AddDeveloperSigningCredential()
                .AddResourceOwnerValidator<ResourceOwnedPasswordValidation>()

                //.AddExtensionGrantValidator<>()
                //.AddExtensionGrantValidator<ExtensionGrantValidator>()
                //.AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>()
                .AddJwtBearerClientAuthentication()

                .AddAppAuthRedirectUriValidator();

            services.AddAuthentication()
                    .AddGoogle("Google", options =>
                    {
                        var settings = services.BuildServiceProvider().GetService<IOptions<GoogleApiOptions>>();
                        //options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        //options.CallbackPath = "/Account/ExternalLoginCallback";
                        options.ClientId = settings.Value.client_id;
                        options.ClientSecret = settings.Value.client_secret;
                    });

            services.AddScoped<IRoleConfigurationDbContext, RoleConfigurationDbContext>();

            services.ConfigureOptions(Configuration);
            services.AddApplicationServices();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
               
                SeedData.EnsureSeedData(serviceScope.ServiceProvider.GetService<IRoleConfigurationDbContext>(), app.ApplicationServices.GetService<IConfiguration>());
            }
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseIdentityServerMongoDBTokenCleanup(applicationLifetime);
            app.UseHttpsRedirection();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

       
    }
}