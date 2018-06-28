using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicIdentityServer.Services;
using Identity.Models;
using Identity.MongoDb;
using Identity.MongoDb.Sample;
using IdentityServer4.MongoDB.Interfaces;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Identity.API;
using Microsoft.eShopOnContainers.Services.Identity.API.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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
            services.AddIdentity<ApplicationUser, IdentityRole>().AddDefaultTokenProviders();
            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                })
                .AddConfigurationStore(Configuration.GetSection("MongoDb"))
                .AddOperationalStore(Configuration.GetSection("MongoDb"))
                .AddDeveloperSigningCredential()
                //.AddExtensionGrantValidator<ExtensionGrantValidator>()
                //.AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>()
                .AddJwtBearerClientAuthentication()
                .AddAppAuthRedirectUriValidator()
                .AddTestUsers(TestUsers.Users);
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));
            services.AddSingleton<IUserStore<ApplicationUser>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoDbSettings>>();
                var client = new MongoClient(options.Value.ConnectionString);
                var database = client.GetDatabase(options.Value.Database);

                return new MongoUserStore<ApplicationUser>(database);
            });
            services.AddTransient<IProfileService, ProfileService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


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
                EnsureSeedData(serviceScope.ServiceProvider.GetService<IConfigurationDbContext>(), app.ApplicationServices.GetService<IConfiguration>());
            }

            app.UseIdentityServer();
            app.UseIdentityServerMongoDBTokenCleanup(applicationLifetime);
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void EnsureSeedData(IConfigurationDbContext context, IConfiguration configuration)
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
        }
    }
}
