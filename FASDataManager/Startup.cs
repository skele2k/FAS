using FASDataManager.Authorization;
using FASLib.DataAccess;
using FASLib.Helpers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace FASDataManager
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                SqliteBaseRepository.CreateDatabase();
            }

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            var myProvider = new AuthorizationServerProvider();
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = false,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(7),
                Provider = myProvider
            };
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }
    }
}