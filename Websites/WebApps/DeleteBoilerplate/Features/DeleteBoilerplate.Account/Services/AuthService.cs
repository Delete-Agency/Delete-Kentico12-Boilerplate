using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CMS.EmailEngine;
using CMS.MacroEngine;
using DeleteBoilerplate.Account.Infrastructure;
using DeleteBoilerplate.Domain.Services;
using Kentico.Membership;
using LightInject;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace DeleteBoilerplate.Account.Services
{
    public class AuthService : IAuthService
    {
        private IOwinContext OwinContext => HttpContext.Current.GetOwinContext();

        private HttpRequest CurrentRequest => HttpContext.Current.Request;

        private UrlHelper UrlHelper => new UrlHelper(CurrentRequest.RequestContext);

        [Inject]
        protected IMailService MailService { get; set; }

        public async Task<IdentityResult> RegisterAsync(AppUser user, string password)
        {
            var identityResult = await UserManager.CreateAsync(user, password);
            if (!identityResult.Succeeded) return identityResult;
            await SignInManager.SignInAsync(user, true, false);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ValidatePasswordResetTokenAsync(int? userId, string token)
        {
            if (!userId.HasValue) return IdentityResult.Failed();
            var isValidToken = await UserManager.VerifyUserTokenAsync(userId.Value, "Confirmation", token);
            return isValidToken ? IdentityResult.Success : IdentityResult.Failed();
        }

        public async Task<IdentityResult> SendPasswordResetConfirmationAsync(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user is null) return IdentityResult.Failed();

            await UserManager.UpdateSecurityStampAsync(user.Id);
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var macro = MacroResolver.GetInstance();
            macro.SetNamedSourceData(nameof(EmailMessage.From), "from@email.com");
            macro.SetNamedSourceData("ConfirmationUrl", ConfirmationUrl(user.Id, token));

            var mailServiceResult = MailService.SendEmail(user, macro, "TemplateName");
            return mailServiceResult ? IdentityResult.Success : IdentityResult.Failed();
        }

        public async Task<IdentityResult> ResetPasswordAsync(int? userId, string newPassword)
        {
            if (!userId.HasValue) return IdentityResult.Failed();
            var user = await UserManager.FindByIdAsync(userId.Value);
            if (user is null) return IdentityResult.Failed();
            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            return await UserManager.ResetPasswordAsync(user.Id, token, newPassword);
        }

        public async Task<AppUser> GetUserInfo()
        {
            if (!AuthManager.User.Identity.IsAuthenticated) return null;
            return await UserManager.FindByNameAsync(AuthManager.User.Identity.Name);
        }

        public async Task<IdentityResult> UpdateUserAsync(AppUser user)
        {
            if (user is null) return IdentityResult.Failed();
            return await UserManager.UpdateAsync(user);
        }

        public async Task<SignInStatus> LoginAsync(string userName, string password, bool isPersistent, bool shouldLockout) =>
            await SignInManager.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);

        public void SignOut() => AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

        private KenticoSignInManager<AppUser> SignInManager => OwinContext.Get<KenticoSignInManager<AppUser>>();

        private KenticoUserManager<AppUser> UserManager => OwinContext.Get<KenticoUserManager<AppUser>>();

        private IAuthenticationManager AuthManager => OwinContext.Authentication;

        private string ConfirmationUrl(int userId, string token) => UrlHelper.Action("ConfirmResetPassword", "Account",
            new { userId, token }, protocol: CurrentRequest.Url.Scheme);
    }
}