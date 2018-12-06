using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Identity.MongoDb;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StsServerIdentity.Filters;
using StsServerIdentity.Models;
using StsServerIdentity.Resources;
using StsServerIdentity.Services;
using StsServerIdentity.Services.Certificate;
using StsServerIdentity.Settings;

namespace StsServerIdentity
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            _environment = env;

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains());
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());

            app.UseCsp(opts => opts
                .BlockAllMixedContent()
                .StyleSources(s => s.Self())
                .StyleSources(s => s.UnsafeInline())
                .FontSources(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(imageSrc => imageSrc.Self())
                .ImageSources(imageSrc => imageSrc.CustomSources("data:"))
                .ScriptSources(s => s.Self())
                .ScriptSources(s => s.UnsafeInline())
            );

            IOptions<RequestLocalizationOptions> locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    if (context.Context.Response.Headers["feature-policy"].Count == 0)
                    {
                        string featurePolicy = "accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'";

                        context.Context.Response.Headers["feature-policy"] = featurePolicy;
                    }

                    if (context.Context.Response.Headers["X-Content-Security-Policy"].Count == 0)
                    {
                        string csp = "script-src 'self';style-src 'self';img-src 'self' data:;font-src 'self';form-action 'self';frame-ancestors 'self';block-all-mixed-content";
                        // IE
                        context.Context.Response.Headers["X-Content-Security-Policy"] = csp;
                    }
                }
            });
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationSection stsConfig = Configuration.GetSection("StsConfig");
            bool useLocalCertStore = Convert.ToBoolean(Configuration["UseLocalCertStore"]);
            string certificateThumbprint = Configuration["CertificateThumbprint"];

            X509Certificate2 cert;

            if (_environment.IsProduction())
            {
                if (useLocalCertStore)
                {
                    using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                    {
                        store.Open(OpenFlags.ReadOnly);
                        X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
                        cert = certs[0];
                        store.Close();
                    }
                }
                else
                {
                    // Azure deployment, will be used if deployed to Azure
                    IConfigurationSection vaultConfigSection = Configuration.GetSection("Vault");
                    var keyVaultService = new KeyVaultCertificateService(vaultConfigSection["Url"], vaultConfigSection["ClientId"], vaultConfigSection["ClientSecret"]);
                    cert = keyVaultService.GetCertificateFromKeyVault(vaultConfigSection["CertificateName"]);
                }
            }
            else
            {
                cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "sts_dev_cert.pfx"), "1234");
            }

            services.Configure<StsConfig>(Configuration.GetSection("StsConfig"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));

            services.AddSingleton<LocService>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddAuthentication()
                  //.AddOpenIdConnect("aad", "Login with Azure AD", options =>
                  //{
                  //    options.Authority = $"https://login.microsoftonline.com/common";
                  //    options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = false };
                  //    options.ClientId = "99eb0b9d-ca40-476e-b5ac-6f4c32bfb530";
                  //    options.CallbackPath = "/signin-oidc";
                  //})
                  .AddGoogle("Google", options =>
                  {
                      IOptions<GoogleApiOptions> settings = services.BuildServiceProvider().GetService<IOptions<GoogleApiOptions>>();
                      //options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                      //options.CallbackPath = "/Account/ExternalLoginCallback";
                      options.ClientId = settings.Value.client_id;
                      options.ClientSecret = settings.Value.client_secret;
                  })
                    .AddFacebook(facebookOptions =>
                    {
                        IOptions<FacebookApiOptions> settings = services.BuildServiceProvider().GetService<IOptions<FacebookApiOptions>>();
                        facebookOptions.AppId = settings.Value.AppId;
                        facebookOptions.AppSecret = settings.Value.AppSecret;
                    });

            services.AddIdentity<MongoIdentityUser, MongoIdentityRole>(config => config.SignIn.RequireConfirmedEmail = true)

                    .AddClaimsPrincipalFactory<ClaimsPrincipalFactory>()
                    .AddDefaultTokenProviders()
                    .AddErrorDescriber<StsIdentityErrorDescriber>()
                    .AddRoles<MongoIdentityRole>()
                    .AddUserStore<MongoUserStore<MongoIdentityUser>>()
                    .AddRoleStore<MongoRoleClaimStore<MongoIdentityRole>>();

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("en-US"),
                            new CultureInfo("de-DE"),
                            new CultureInfo("de-CH"),
                            new CultureInfo("it-IT"),
                            new CultureInfo("gsw-CH"),
                            new CultureInfo("fr-FR"),
                            new CultureInfo("zh-Hans")
                        };

                    options.DefaultRequestCulture = new RequestCulture(culture: "de-DE", uiCulture: "de-DE");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;

                    var providerQuery = new LocalizationQueryProvider
                    {
                        QureyParamterName = "ui_locales"
                    };

                    options.RequestCultureProviders.Insert(0, providerQuery);
                });

            services.AddMvc(options =>
            {
                options.Filters.Add(new SecurityHeadersAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("SharedResource", assemblyName.Name);
                    };
                });

            services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IRoleConfigurationDbContext, RoleConfigurationDbContext>();
            services.AddIdentityServer()
                .AddSigningCredential(cert)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients(stsConfig))
                .AddAspNetIdentity<MongoIdentityUser>()
                .AddProfileService<IdentityWithAdditionalClaimsProfileService>();

            services.AddApplicationServices();
        }
    }
}