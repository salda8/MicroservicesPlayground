using BasicIdentityServer.Configuration;
using Identity.MongoDb;
using IdentityServer4.MongoDB.Mappers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityServer.Services
{
    public class SeedData
    {
       internal static void EnsureSeedData(IRoleConfigurationDbContext context, IConfiguration configuration)
        {
            if (!context.Clients.Any())
            {
                var clientUrls = new Dictionary<string, string>();

                clientUrls.Add("AppointmentApi", configuration.GetValue<string>("AppointmentApi"));
                clientUrls.Add("Mvc", configuration.GetValue<string>("MvcTest"));
                clientUrls.Add("PaymentApi", configuration.GetValue<string>("PaymentApi"));

                foreach (var client in Config.GetClients(clientUrls).ToList())
                {
                    context.AddClient(client.ToEntity());
                }
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetResources().ToList())
                {
                    context.AddIdentityResource(resource.ToEntity());
                }
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.GetApis().ToList())
                {
                    context.AddApiResource(resource.ToEntity());
                }
            }

            if (!context.Roles.Any())
            {
                foreach (var resource in Config.GetRoles().ToList())
                {
                    var res = resource;
                    res.NormalizedName = res.Name.ToUpperInvariant();
                    context.AddRolesAsync(res);
                }
            }
        }
    }
}
