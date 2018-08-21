using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using CacheManager.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;
using Ocelot.Administration;

namespace OcelotApiGw
{
    public class Startup
    {

        private readonly IConfiguration _cfg;

        public Startup(IConfiguration configuration)
        {
            _cfg = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var identityUrl = _cfg.GetValue<string>("IdentityUrl");
            var authenticationProviderKey = "IdentityApiKey";

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddMvc();//.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddAuthentication().AddOath2();


            Action<IdentityServer4.AccessTokenValidation.IdentityServerAuthenticationOptions> configureOptions = options =>
              {
                  options.Authority = identityUrl;
                  options.RequireHttpsMetadata = false;
                  options.ApiName = "gateway";
                  options.EnableCaching = true;
                  options.CacheDuration = TimeSpan.FromMinutes(10);

              };
            services.AddAuthentication("Bearer").AddIdentityServerAuthentication("test", configureOptions);

            services.AddOcelot(_cfg).AddEureka().AddAdministration("/administration", configureOptions);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = _cfg["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(_cfg.GetSection("Logging"));
            app.UseAuthentication();
            app.UseMvc();
            app.UseCors("CorsPolicy");

            app.UseOcelot().Wait();
        }

        /// <summary>
        /// Method for overriding default ocelot middlewares
        /// </summary>
        /// <returns></returns>
        public OcelotPipelineConfiguration OcelotPipelineConfiguration()
        {
            var pipelineConfiguration = new OcelotPipelineConfiguration();

            pipelineConfiguration.AuthorisationMiddleware = async (ctx, next) =>
                {

                    await next.Invoke();
                };



            return pipelineConfiguration;

        }
    }
}
