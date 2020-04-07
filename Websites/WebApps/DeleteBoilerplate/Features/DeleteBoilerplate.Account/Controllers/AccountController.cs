using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using DeleteBoilerplate.Account.Infrastructure;
using DeleteBoilerplate.Account.Models;
using DeleteBoilerplate.Account.Services;
using LightInject;
using Microsoft.AspNet.Identity.Owin;

namespace DeleteBoilerplate.Account.Controllers
{
    public class AccountController : Controller
    {
        [Inject]
        protected IAuthService AuthService { get; set; }

        [Inject]
        protected IMapper Mapper { get; set; }


        [HttpGet]
        public ActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account/Register.cshtml");
            var user = Mapper.Map<AppUser>(model);
            var identityResult = await AuthService.RegisterAsync(user, model.Password);
            var result = identityResult.Succeeded ? "Ok" : string.Join(",", identityResult.Errors);
            return Content(result, "application/json");
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            AuthService.SignOut();
            return Content("Sign out successful", "application/json");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account/Login.cshtml");
            var signInStatus = await AuthService.LoginAsync(model.Email, model.Password, true, false);
            var result = signInStatus == SignInStatus.Failure ? "Error" : "Ok";
            return Content(result, "application/json");
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmResetPassword(int? userId, string token)
        {
            var validationResult = await AuthService.ValidatePasswordResetTokenAsync(userId, token);
            var result = validationResult.Succeeded ? "Ok" : string.Join(",", validationResult.Errors);
            return Content(result, "");
        }
    }
}