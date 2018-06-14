using AppointmentApi;
using AppointmentApi.AppointmentModel.ValueObjects;
using Autofac;
using Autofac.Extensions;
using Autofac.Extensions.DependencyInjection;
using EventFlow;
using EventFlow.Autofac.Extensions;
using EventFlow.Extensions;
using EventFlow.Logs;
using EventFlow.MongoDB.Extensions;
using EventFlow.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Swashbuckle.AspNetCore.Swagger;
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
});
            
            
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
            
            BsonClassMap.RegisterClassMap<ValueObject>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<SingleValueObject<string>>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<SingleValueObject<Location>>(cm =>
           {
               cm.AutoMap();
           });
            BsonClassMap.RegisterClassMap<AppointmentReadModel>(cm =>
            {
                //cm.AutoMap();
                var bcm = cm.BaseClassMap;
                cm.MapIdMember(x => x.Id);
                //cm.AutoMap();
            });

            

            // Add any Autofac modules or registrations.
            // This is called AFTER ConfigureServices so things you
            // register here OVERRIDE things registered in ConfigureServices.
            //
            // You must have the call to AddAutofac in the Program.Main
            // method or this won't be called.

            builder.RegisterModule(new AutofacModule());
            var container = EventFlowOptions.New
              .UseAutofacContainerBuilder(builder) // Must be the first line!
              .AddDefaults(Assembly.GetExecutingAssembly())
              .UseMongoDbReadModel<AppointmentReadModel>()
              .UseMongoDbInsertOnlyReadModel<AppointmentInsertReadModel>()
              .UseMongoDbEventStore()
              .ConfigureMongoDb("mongodb://localhost:27017", "test");

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
            builder.Populate(new ServiceCollection());
            
        }
    }

}