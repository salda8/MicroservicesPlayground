using Identity.MongoDb;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace BasicIdentityServer.Configuration
{
    public class Config
    {
        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API"),
                new ApiResource("appointment", "Appointment Service"),
                new ApiResource("payment", "Payment Service"),
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                // JavaScript Client
                new Client
        {
            ClientId = "ro.client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "appointment" }
        },
                new Client
                {
                    ClientId = "appointmentService",
                    //ClientName = "Appointment Service",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                   // ClientUri = $"{clientsUrl["AppointmentApi"]}",                             // public uri of the client
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //AllowAccessTokensViaBrowser = false,
                    //RequireConsent = false,
                    //AllowOfflineAccess = true,
                    //AlwaysIncludeUserClaimsInIdToken = true,
                    //RedirectUris = new List<string>
                    //{
                    //    $"{clientsUrl["AppointmentApi"]}/signin-oidc"
                    //},
                    //PostLogoutRedirectUris = new List<string>
                    //{
                    //    $"{clientsUrl["AppointmentApi"]}/signout-callback-oidc"
                    //},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "appointment",
                        "payment",
                    },
                },
                new Client
                {
                    ClientId = "mvctest",
                    //ClientName = "MVC Client Test",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    //ClientUri = $"{clientsUrl["Mvc"]}",                             // public uri of the client
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //AllowAccessTokensViaBrowser = true,
                    //RequireConsent = false,
                    //AllowOfflineAccess = true,
                    //RedirectUris = new List<string>
                    //{
                    //    $"{clientsUrl["Mvc"]}/signin-oidc"
                    //},
                    //PostLogoutRedirectUris = new List<string>
                    //{
                    //    $"{clientsUrl["Mvc"]}/signout-callback-oidc"
                    //},
                    AllowedScopes = new List<string>
                    {
                        //IdentityServerConstants.StandardScopes.OpenId,
                        //IdentityServerConstants.StandardScopes.Profile,
                        //IdentityServerConstants.StandardScopes.OfflineAccess,

                        "appointment"
                    }},
                new Client
                {
                    ClientId = "payment",
                    ClientName = "Payment Api Service",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    //ClientUri = $"{clientsUrl["PaymentApi"]}",                             // public uri of the client
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //AllowAccessTokensViaBrowser = true,
                    //RequireConsent = false,
                    //AllowOfflineAccess = true,
                    //RedirectUris = new List<string>
                    //{
                    //    $"{clientsUrl["PaymentApi"]}/signin-oidc"
                    //},
                    //PostLogoutRedirectUris = new List<string>
                    //{
                    //    $"{clientsUrl["PaymentApi"]}/signout-callback-oidc"
                    //},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                    },
                },
             new Client
        {
            ClientId = "client",

            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ClientCredentials,

            // secret for authentication
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },

            // scopes that client has access to
            AllowedScopes = { "api1" }
        }
};
        }

        internal static IEnumerable<MongoIdentityRole> GetRoles() {

            var userRole = new MongoIdentityRole("user");
            userRole.Claims.Add(new Identity.MongoDb.Models.MongoUserClaim(new System.Security.Claims.Claim(ClaimTypes.GroupSid, Guid.NewGuid().ToString())));
            var roles=  new List<MongoIdentityRole>()
            {
               userRole,
                new MongoIdentityRole("administrator"),
                new MongoIdentityRole("customer")
            };

            return roles;
        }
    }
}