using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace OHEXML.Infrastructure.IdentityServer4
{
    public class Config
    {
        /// <summary>
        /// 资源集合(定义的每一个资源可理解为一个程序)
        /// </summary>
        public static IEnumerable<ApiResource> ApiSources =>
        new List<ApiResource>
        {           
            new ApiResource
            {
                Name="OHEXMLAPI",//对应Audience(受众)
                UserClaims=new string[]
                {
                    JwtClaimTypes.Id,
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Role
                },
                Scopes=new Scope[]
                {
                    new Scope("client"),
                    new Scope("doctor"),
                    new Scope("background"),
                }
            },
            new ApiResource
            {
                Name="TestOHEXMLAPI",//对应Audience(受众)
                UserClaims=new string[]
                {
                    JwtClaimTypes.Id,
                    JwtClaimTypes.Name,
                    JwtClaimTypes.Role
                },
                Scopes=new Scope[]
                {
                    new Scope("test"),
                }
            }
        };

        /// <summary>
        /// 客服端集合(用于访问API资源的群体)
        /// </summary>
        public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "Sunflower",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AccessTokenLifetime=3600*24*30,//有效时间
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = { "client","doctor","background" }//允许请求范围
            }
        };
    }
}
