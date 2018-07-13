using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDbGenericRepository;
using SettingsApi.Repository;

namespace SettingsApi
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
            services.Configure<MongoConfigurationOptions>(Configuration.GetSection("MongoDb"));
            services.AddScoped<IGenericMongoRepository, GenericMongoRepository>();
            services.AddScoped<IBaseMongoRepository, GenericMongoRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options =>
            {
                options.Authority = Configuration.GetSection("IdentityUrlExternal").Value;
                options.RequireHttpsMetadata = false;
                options.ApiName = "settings";

            });

            services.AddSwaggerDocumentation(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandlingMiddleware();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandlingMiddleware();
            }


            
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
