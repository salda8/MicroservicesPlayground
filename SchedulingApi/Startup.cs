using AppointmentApi;
using AppointmentApi.AppointmentModel.ValueObjects;
using Autofac;
using Autofac.Extensions;
using Autofac.Extensions.DependencyInjection;
using EventFlow;
using EventFlow.Autofac.Extensions;
using EventFlow.Extensions;
using EventFlow.Kafka;
using EventFlow.Logs;
using EventFlow.MongoDB.Extensions;
using EventFlow.RabbitMQ;
using EventFlow.RabbitMQ.Extensions;
using EventFlow.Snapshots.Strategies;
using EventFlow.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using SchedulingApi.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using System;
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
            ConfigureIdentity(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options =>
            {
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.ApiName = "appointment";
            });
            
            services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
});
            
            
        }

        private void ConfigureIdentity(IServiceCollection services) {
            


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseExceptionHandlingMiddleware();
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandlingMiddleware();
            }
            else
            {

                app.UseHsts();
            }


            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
            // var resolver = EventFlowOptions.New.AddDefaults(Assembly.GetExecutingAssembly()).CreateResolver();
            // var container = EventFlowOptions.New
            //.UseAutofacContainerBuilder(containerBuilder) // Must be the first line!
            //.AddDefaults(Assembly.GetExecutingAssembly())
            //.CreateResolver();
            //.CreateContainer();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {

            BsonClassMap.RegisterClassMap<SingleValueObject<string>>(cm =>
            {
                cm.MapMember(x => x.Value);
                //cm.SetIsRootClass(true);

            });
            BsonClassMap.RegisterClassMap<SingleValueObject<DateTime>>(cm =>
           {
               cm.MapMember(x => x.Value);
                //cm.SetIsRootClass(true);

            });
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



            // Add any Autofac modules or registrations.
            // This is called AFTER ConfigureServices so things you
            // register here OVERRIDE things registered in ConfigureServices.
            //
            // You must have the call to AddAutofac in the Program.Main
            // method or this won't be called.
            var amqpuri = new Uri("amqp://localhost");
            var kafkaUri = "127.0.0.1:9092";
            builder.RegisterModule(new AutofacModule());
            var container = EventFlowOptions.New
              .UseAutofacContainerBuilder(builder) // Must be the first line!
              .AddDefaults(Assembly.GetExecutingAssembly())
              
              .UseMongoDbReadModel<AppointmentReadModel>()
              .UseMongoDbInsertOnlyReadModel<AppointmentInsertReadModel>()
              .UseMongoDbEventStore()
              .UseMongoDbSnapshotStore()
              .PublishToKafka(KafkaConfiguration.With(kafkaUri))

              .ConfigureMongoDb("mongodb://localhost:27017", "test");
            container.UseLibLog(LibLogProviders.NLog);

             // .UseIndMemoryReadStoreFor<AppointmentReadModel>()
        }

    }

    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // The generic ILogger<TCategoryName> service was added to the ServiceCollection by ASP.NET Core.
            // It was then registered with Autofac using the Populate method in ConfigureServices.
            builder.RegisterType<MyLogger>().As<ILog>();
            builder.RegisterType<ExceptionHandlingMiddleware>();
            builder.RegisterType<AppointmentService>().As<IAppointmentService>();
            ISnapshotStrategy newStrategy = SnapshotEveryFewVersionsStrategy.With(10);
            builder.RegisterInstance(newStrategy);
          // builder.RegisterType<EventFlowOptionsSnapshotExtensions>
            builder.Populate(new ServiceCollection());
            
        }
    }

}