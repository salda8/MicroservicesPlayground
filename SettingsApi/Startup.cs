using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbGenericRepository;
using SettingsApi.Repository;
using Steeltoe.Discovery.Client;

namespace SettingsApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

            app.UseDiscoveryClient();
        }

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
            services.AddDiscoveryClient(Configuration);
            services.AddSwaggerDocumentation(Configuration);
        }
    }
}