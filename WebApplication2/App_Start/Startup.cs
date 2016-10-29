using Owin;

using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

[assembly:OwinStartup(typeof(WebApplication2.App_Start.Startup))]
namespace WebApplication2.App_Start
{
    public class Startup
    {
        private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string _aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string _tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string _postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        string _authority = String.Format(CultureInfo.InvariantCulture, _aadInstance, _tenant);

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions {
                ClientId = _clientId,
                Authority = _authority,
                PostLogoutRedirectUri = _postLogoutRedirectUri,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context => {
                        context.HandleResponse();
                        context.Response.Redirect("/Error/message=" + context.Exception.Message);
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}