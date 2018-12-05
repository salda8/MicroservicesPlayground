using System.Reflection;
using AppointmentApi;
using AppointmentApi.AppointmentModel.ValueObjects;
using AppointmentApi.Models.Appointment.Integration;
using Autofac;
using EventBus.Kafka;
using EventFlow;
using EventFlow.Autofac.Extensions;
using EventFlow.Extensions;
using EventFlow.Kafka.Configuration;
using EventFlow.Kafka.Extensions;
using EventFlow.MongoDB;
using EventFlow.MongoDB.Extensions;
using Identity.MongoDb;
using MicroservicesPlayground.EventBus;
using MicroservicesPlayground.EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using Payments.Domain.Payments.ReadModels;
using SchedulingApi.Controllers;
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

            string amqpUri = Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>().ConnectionString;
            System.Collections.Generic.List<string> kafkaUri = Configuration.GetSection("Kafka").Get<KafkaSettings>().BrokerAddresses;
            MongoDbSettings mongoDbSettings = Configuration.GetSection("MongoDb").Get<MongoDbSettings>();
            IEventFlowOptions container = EventFlowOptions.New
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

            //services.AddHealthChecks(checks =>
            //{
            //    checks.AddValueTaskCheck("HTTP Endpoint", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            //});

            services.AddSingleton<ISubscriptionEventBus, EventBusKafka>(sp =>
                {
                    ILifetimeScope iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    IKafkaConsumerFactory consumerFactory = sp.GetRequiredService<IKafkaConsumerFactory>();
                    KafkaSettings kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
                    IOptions<KafkaConsumerConfiguration> kafkaConfig = Options.Create(new KafkaConsumerConfiguration(kafkaSettings.BrokerAddresses, kafkaSettings.GroupId,
                    kafkaSettings.ClientId, kafkaSettings.SubscribedTopics));
                    System.Collections.Generic.List<string> topics = kafkaSettings.SubscribedTopics;
                    IEventBusSubscriptionsManager eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    return new EventBusKafka(eventBusSubscriptionsManager, consumerFactory, iLifetimeScope, kafkaConfig);
                });

            services.AddDiscoveryClient(Configuration);
            services.AddSwaggerDocumentation(Configuration);
            services.AddTransient<IntegrationTestEventHandler>();
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
                cm.MapCreator(cs => new CarService(cs.Name, cs.Price));
            });
            BsonClassMap.RegisterClassMap<AppointmentInsertReadModel>(cm =>
            {
                cm.AutoMap();
            });

            // var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            // ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            ISubscriptionEventBus eventBus = app.ApplicationServices.GetRequiredService<ISubscriptionEventBus>();
            eventBus.Subscribe<LocationSet, IntegrationTestEventHandler>();
        }
    }
}