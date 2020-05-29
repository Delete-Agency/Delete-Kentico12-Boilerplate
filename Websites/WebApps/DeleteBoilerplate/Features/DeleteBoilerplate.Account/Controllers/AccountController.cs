using CMS.Helpers;
using DeleteBoilerplate.Account.Infrastructure;
using DeleteBoilerplate.Account.Models;
using DeleteBoilerplate.Account.Services;
using DeleteBoilerplate.Common.Helpers;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.Infrastructure.Models.FormComponents.ValidationError;
using LightInject;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeleteBoilerplate.Account.Controllers
{
    public class AccountController : BaseApiController
    {
        [Inject]
        protected IAuthService AuthService { get; set; }

        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    var validationErrors = ValidationErrorModel.Build(this.ModelState);
                    return JsonError(data: validationErrors);
                }

                var signInStatus = await this.AuthService.LoginAsync(model.Email, model.Password, true, false);

                return signInStatus == SignInStatus.Success
                    ? JsonSuccess()
                    : JsonError(ResHelper.GetString("DeleteBoilerplate.Account.Login.Error"));
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                return JsonError();
            }
        }

        [HttpPost]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            try
            {
                if (this.ModelState.IsValid == false)
                {
                    var validationErrors = ValidationErrorModel.Build(this.ModelState);
                    return JsonError(data: validationErrors);
                }

                var user = this.Mapper.Map<AppUser>(model);
                var identityResult = await this.AuthService.RegisterAsync(user, model.Password);

                return identityResult.Succeeded
                    ? JsonSuccess()
                    : JsonError(ResHelper.GetString("DeleteBoilerplate.Account.Register.Error"));
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                return JsonError();
            }
        }

        [HttpPut]
        public async Task<JsonResult> ResetPassword(int? userId, string token)
        {
            try
            {
                var validationResult = await this.AuthService.ValidatePasswordResetTokenAsync(userId, token);

                return validationResult.Succeeded
                    ? JsonSuccess()
                    : JsonError(ResHelper.GetString("DeleteBoilerplate.Account.ResetPassword.Error"));
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                return JsonError();
            }
        }

        [HttpDelete]
        public ActionResult Logout()
        {
            try
            {
                this.AuthService.SignOut();
                return JsonSuccess();
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                return JsonError();
            }
        }
    }
}