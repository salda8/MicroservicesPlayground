using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicIdentityServer.Services;

using Identity.MongoDb;

using IdentityServer4.MongoDB.Interfaces;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BasicIdentityServer;
using BasicIdentityServer.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using BasicIdentityServer.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.Validation;

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
            //     services.AddIdentityServer()
            //.AddDeveloperSigningCredential()
            //.AddInMemoryApiResources(Config.GetApis())
            //.AddInMemoryClients(Config.GetClients(new Dictionary<string, string>()));
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddIdentity<MongoIdentityUser>().AddClaimsPrincipalFactory<ClaimsPrincipalFactory>().AddDefaultTokenProviders();
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
                //.AddTestUsers(TestUsers.Users);
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));
            services.AddSingleton<IUserStore<MongoIdentityUser>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbSettings>>();
                var client = new MongoClient(options.Value.ConnectionString);
                var database = client.GetDatabase(options.Value.Database);

                return new MongoUserStore<MongoIdentityUser>(database);
            });


            var roleStore = CreateMongoRoleClaimStore(services);

            services.AddSingleton<IRoleStore<MongoIdentityRole>>(roleStore);
            services.AddSingleton<IRoleClaimStore<MongoIdentityRole>>(roleStore);
            //services.AddSingleton<IRoleClaimStore<MongoIdentityRole>>(provider =>
            //{
            //    var options = provider.GetService<IOptions<MongoDbSettings>>();
            //    var client = new MongoClient(options.Value.ConnectionString);
            //    var database = client.GetDatabase(options.Value.Database);

            //    return new MongoRoleClaimStore<MongoIdentityRole>(database);
            //});

            services.AddScoped<RoleManager<MongoIdentityRole>>();
            services.AddScoped<IRoleConfigurationDbContext, RoleConfigurationDbContext>();
           
            //services.AddTransient<IRoleStore, RoleStore<>>
            services.AddTransient<IRedirectService, RedirectService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<ILoginService<MongoIdentityUser>, LoginService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


        }

        public static MongoRoleClaimStore<MongoIdentityRole> CreateMongoRoleClaimStore(IServiceCollection services)
        {
            var options = services.BuildServiceProvider().GetService<IOptions<MongoDbSettings>>();
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.Database);
            return new MongoRoleClaimStore<MongoIdentityRole>(database);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IApplicationLifetime applicationLifetime)
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
                EnsureSeedData(serviceScope.ServiceProvider.GetService<IRoleConfigurationDbContext>(), app.ApplicationServices.GetService<IConfiguration>());
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

        private void EnsureSeedData(IRoleConfigurationDbContext context, IConfiguration configuration)
        {
            if (!context.Clients.Any())
            {
                var clientUrls = new Dictionary<string, string>();

                clientUrls.Add("AppointmentApi", configuration.GetValue<string>("AppointmentApi"));
                clientUrls.Add("Mvc", configuration.GetValue<string>("MvcTest"));
                clientUrls.Add("PaymentApi", configuration.GetValue<string>("PaymentApi"));



                foreach (var client in Config.GetClients(clientUrls).ToList())
                {
                    context.AddClient(client.ToEntity());
                }
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetResources().ToList())
                {
                    context.AddIdentityResource(resource.ToEntity());
                }
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.GetApis().ToList())
                {
                    context.AddApiResource(resource.ToEntity());
                }
            }

            if (!context.Roles.Any())
            {
                foreach (var resource in Config.GetRoles().ToList())
                {
                    context.AddRolesAsync(resource);
                }
            }
        }
    }

    
}
