using System.Web.Mvc;

namespace DeleteBoilerplate.Account.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }
    }
}