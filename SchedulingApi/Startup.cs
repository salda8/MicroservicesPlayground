using AppointmentApi;
using AppointmentApi.Models.Appointment.Integration;
using AppointmentApi.AppointmentModel.ValueObjects;
using Autofac;
using MicroservicesPlayground.EventBus.Abstractions;
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
using EventBus.Kafka;
using EventBus;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Options;
using Payments.Domain.Payments.ReadModels;
using MongoDB.Bson.Serialization.Conventions;
using EventFlow.MongoDB;
using EventFlow.Kafka.Configuration;
using MicroservicesPlayground.EventBus;
using EventFlow.Kafka.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using Steeltoe.Discovery.Client;

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
            services.Configure<KafkaSettings>(Configuration.GetSection("Kafka"));
           
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options =>
            {
                options.Authority = Configuration.GetSection("IdentityUrlExternal").Value;
                options.RequireHttpsMetadata = false;
                options.ApiName = "appointment";

            });

            services.AddSingleton<ISubscriptionEventBus, EventBusKafka>(sp =>
                {
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var consumerFactory = sp.GetRequiredService<IKafkaConsumerFactory>();
                    var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
                    var kafkaConfig = new KafkaConsumerConfiguration(kafkaSettings.BrokerAddresses, kafkaSettings.GroupId, 
                    kafkaSettings.ClientId, kafkaSettings.SubscribedTopics);
                    var valueDeserializer = new StringDeserializer(System.Text.Encoding.Default);
                    var topics = kafkaSettings.SubscribedTopics;
                    var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    return new EventBusKafka(eventBusSubscriptionsManager, consumerFactory, iLifetimeScope, kafkaConfig, valueDeserializer);
                });

            services.AddDiscoveryClient(Configuration);
            services.AddSwaggerDocumentation(Configuration);
            services.AddTransient<IntegrationTestEventHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
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

            app.UseDiscoveryClient();
            app.UseSwaggerDocumentation();

            ConfigureEventBus(app);
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

            var amqpUri = Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>().ConnectionString;
            var kafkaUri = Configuration.GetSection("Kafka").Get<KafkaSettings>().BrokerAddresses;
            var mongoDbSettings = Configuration.GetSection("MongoDb").Get<MongoDbSettings>();
            var container = EventFlowOptions.New
              .UseAutofacContainerBuilder(builder) // Must be the first line!
              .AddDefaults(Assembly.GetExecutingAssembly())
              .UseMongoDbReadModel<AppointmentReadModel>()
              .UseMongoDbInsertOnlyReadModel<AppointmentInsertReadModel>()
              .UseMongoDbReadModel<PaymentDetailsReadModel>()
              .UseMongoDbInsertOnlyReadModel<PaymentDetailsReadModel>()

              .UseMongoDbEventStore()
              .UseMongoDbSnapshotStore()
              
              .PublishToKafka(KafkaConfiguration.With(kafkaUri))
              .UseLibLog(LibLogProviders.Serilog)
              .ConfigureMongoDb(mongoDbSettings.ConnectionString, mongoDbSettings.Database);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<ISubscriptionEventBus>();
            eventBus.Subscribe<LocationSet, IntegrationTestEventHandler>();

        }



        private static void BsonMapping()
        {
            BsonClassMapping.RegisterClassMaps();

            BsonClassMap.RegisterClassMap<AppointmentId>(cm =>
            {
                //cm.AutoMap();
                cm.MapCreator(x => new AppointmentId(x.Value));
            });
            BsonClassMap.RegisterClassMap<AppointmentApi.AppointmentModel.ValueObjects.Location>(cm =>
            {
                //cm.AutoMap();
                cm.MapCreator(x => new AppointmentApi.AppointmentModel.ValueObjects.Location(x.Value));
            });
            BsonClassMap.RegisterClassMap<Schedule>(cm =>
            {
                //cm.AutoMap();
                cm.MapCreator(x => new Schedule(x.Value));
            });
            BsonClassMap.RegisterClassMap<CarService>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(cs=> new CarService(cs.Name, cs.Price));
            });
            BsonClassMap.RegisterClassMap<AppointmentInsertReadModel>(cm=>{
                cm.AutoMap();
            });

            // var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            // ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

        }
    }
}