using AppointmentApi;
using AppointmentApi.AppointmentModel.ValueObjects;
using Autofac;
using EventFlow;
using EventFlow.Autofac.Extensions;
using EventFlow.Extensions;
using EventFlow.Kafka;
using EventFlow.MongoDB.Extensions;
using EventFlow.RabbitMQ.Extensions;
using Identity.MongoDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using SchedulingApi.Controllers;
using System.Reflection;

namespace SchedulingApi
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options =>
            {
                options.Authority = Configuration.GetSection("IdentityUrlExternal").Value;
                options.RequireHttpsMetadata = false;
                options.ApiName = "appointment";
                
            });

            services.AddSwaggerDocumentation(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseExceptionHandlingMiddleware();
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandlingMiddleware();
            }
            else
            {
                app.UseHsts();
            }

            app.UseExceptionHandlingMiddleware();

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseMvc();
            app.UseCors(builder =>
            {
              builder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials();
            });

            app.UseSwaggerDocumentation();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            BsonMapping();

            // Add any Autofac modules or registrations.
            // This is called AFTER ConfigureServices so things you
            // register here OVERRIDE things registered in ConfigureServices.
            //
            // You must have the call to AddAutofac in the Program.Main
            // method or this won't be called.

            builder.RegisterModule(new AutofacModule());
            var amqpuri = Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>().ConnectionString;
            var kafkaUri = Configuration.GetSection("Kafka").Get<KafkaSettings>().ConnectionString;
            var mongoDbSettings = Configuration.GetSection("MongoDb").Get<MongoDbSettings>();
            var container = EventFlowOptions.New
              .UseAutofacContainerBuilder(builder) // Must be the first line!
              .AddDefaults(Assembly.GetExecutingAssembly())
              .UseMongoDbReadModel<AppointmentReadModel>()
              .UseMongoDbInsertOnlyReadModel<AppointmentInsertReadModel>()
              .UseMongoDbEventStore()
              .UseMongoDbSnapshotStore()
              .PublishToKafka(KafkaConfiguration.With(kafkaUri))
              .UseLibLog(LibLogProviders.Serilog)
              .ConfigureMongoDb(mongoDbSettings.ConnectionString, mongoDbSettings.Database);
        }

        private static void BsonMapping()
        {
            BsonClassMap.RegisterClassMap<AppointmentId>(cm =>
            {
                cm.MapCreator(x => new AppointmentId(x.Value));
            });
            BsonClassMap.RegisterClassMap<Location>(cm =>
            {
                cm.MapCreator(x => new Location(x.Value));
            });
            BsonClassMap.RegisterClassMap<Schedule>(cm =>
            {
                cm.MapCreator(x => new Schedule(x.Value));
            });
        }
    }
}