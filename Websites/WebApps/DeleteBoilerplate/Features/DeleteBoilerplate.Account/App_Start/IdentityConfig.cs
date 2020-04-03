using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using CMS.Helpers;
using CMS.SiteProvider;
using Kentico.Membership;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using DeleteBoilerplate.Account.Infrastructure;
using Microsoft.Owin.Infrastructure;


namespace DeleteBoilerplate.Account
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<KenticoUserManager<AppUser>>(() =>
            {
                var userManager = new KenticoUserManager<AppUser>(new KenticoUserStore<AppUser>(SiteContext.CurrentSiteName));
                userManager.UserValidator = new UserValidator<AppUser, int>(userManager)
                {
                    AllowOnlyAlphanumericUserNames = true,
                    RequireUniqueEmail = true
                };
                var provider = new DpapiDataProtectionProvider(Domain.Constants.Identity.AppName);
                userManager.UserTokenProvider =
                    new DataProtectorTokenProvider<AppUser, int>(provider.Create(Domain.Constants.Identity.AppTokenName)) { TokenLifespan = TimeSpan.FromDays(1)};
                userManager.EmailService = new EmailService();
                return userManager;
            });

            app.CreatePerOwinContext<KenticoSignInManager<AppUser>>(KenticoSignInManager<AppUser>.Create);

            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = DeleteBoilerplate.Domain.Constants.Identity.AppCookieName,
                CookieSecure = CookieSecureOption.SameAsRequest,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(urlHelper.Action("Login", "Account")),
                CookieManager = new SystemWebCookieManager(),
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = context =>
                        context.Response.Redirect(urlHelper.Action("Login", "Account") + new Uri(context.RedirectUri).Query)
                }
            });
            CookieHelper.RegisterCookie(DefaultAuthenticationTypes.ApplicationCookie, CookieLevel.Essential);
        }
    }
}