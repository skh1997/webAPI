using System.Collections.Generic;
using ApiSettings;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityModel;

namespace AuthorizationServer.Configuration
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(SalesApiSettings.ApiName, SalesApiSettings.ApiDisplayName) {
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // Sales JavaScript Client
                new Client
                {
                    ClientId = SalesApiSettings.ClientId,
                    ClientName = SalesApiSettings.ClientName,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 60 * 10,
                    AllowOfflineAccess = true,
                    RedirectUris =           { $"{Startup.Configuration["SalesApi:ClientBase"]}/login-callback", $"{Startup.Configuration["SalesApi:ClientBase"]}/silent-renew.html" },
                    PostLogoutRedirectUris = { Startup.Configuration["SalesApi:ClientBase"] },
                    AllowedCorsOrigins =     { Startup.Configuration["SalesApi:ClientBase"] },
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        SalesApiSettings.ApiName
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
    }
}
