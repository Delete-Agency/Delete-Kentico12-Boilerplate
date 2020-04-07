using System.Threading.Tasks;
using DeleteBoilerplate.Account.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DeleteBoilerplate.Account.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(AppUser user, string password);

        Task<SignInStatus> LoginAsync(string userName, string password, bool isPersistent, bool shouldLockout);

        Task<IdentityResult> ResetPasswordAsync(int? userId, string newPassword);

        Task<IdentityResult> SendPasswordResetConfirmationAsync(string email);

        Task<IdentityResult> ValidatePasswordResetTokenAsync(int? userId, string token);

        Task<AppUser> GetUserInfo();

        Task<IdentityResult> UpdateUserAsync(AppUser user);

        void SignOut();
    }
}