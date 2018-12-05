using System;
using System.IdentityModel.Tokens.Jwt;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus.Kafka;
using EventFlow.Kafka.Configuration;
using MicroservicesPlayground.EventBus;
using MicroservicesPlayground.EventBus.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.SignalrHub.AutofacModules;
using Ordering.SignalrHub.IntegrationEvents.EventHandling;
using Ordering.SignalrHub.IntegrationEvents.Events;

namespace Ordering.SignalrHub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationsHub>("/notificationhub", options =>
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
            });

            ConfigureEventBus(app);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddSignalR();

            ConfigureAuthService(services);

            RegisterEventBus(services);

            services.AddOptions();

            //configure autofac
            var container = new ContainerBuilder();
            container.RegisterModule(new ApplicationModule());
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            string identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "orders.signalrhub";
            });
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            ISubscriptionEventBus eventBus = app.ApplicationServices.GetRequiredService<ISubscriptionEventBus>();
            eventBus.Subscribe<ScheduleConfirmedEvent, ScheduleConfirmedEventHandler>();
            // eventBus.Subscribe<OrderStatusChangedToAwaitingValidationIntegrationEvent, OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
            // eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
            // eventBus.Subscribe<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
            // eventBus.Subscribe<OrderStatusChangedToShippedIntegrationEvent, OrderStatusChangedToShippedIntegrationEventHandler>();
            // eventBus.Subscribe<OrderStatusChangedToCancelledIntegrationEvent, OrderStatusChangedToCancelledIntegrationEventHandler>();
            // eventBus.Subscribe<OrderStatusChangedToSubmittedIntegrationEvent, OrderStatusChangedToSubmittedIntegrationEventHandler>();
        }

        private void RegisterEventBus(IServiceCollection services)
        {
            services.Configure<KafkaSettings>(Configuration.GetSection("Kafka"));

            services.AddSingleton<ISubscriptionEventBus, EventBusKafka>(sp =>
                {
                    ILifetimeScope iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    IKafkaConsumerFactory consumerFactory = sp.GetRequiredService<IKafkaConsumerFactory>();
                    IOptions<KafkaConsumerConfiguration> kafkaSettings = sp.GetRequiredService<IOptions<KafkaConsumerConfiguration>>();

                    IEventBusSubscriptionsManager eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    return new EventBusKafka(eventBusSubscriptionsManager, consumerFactory, iLifetimeScope, kafkaSettings);
                });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
}