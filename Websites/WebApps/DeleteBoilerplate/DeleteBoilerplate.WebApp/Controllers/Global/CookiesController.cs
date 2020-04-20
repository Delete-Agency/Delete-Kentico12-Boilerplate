using CMS.EventLog;
using CMS.Helpers;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.WebApp.Constants;
using DeleteBoilerplate.WebApp.Models.Global.Cookies;
using LightInject;
using System;
using System.Web;
using System.Web.Mvc;

namespace DeleteBoilerplate.WebApp.Controllers.Global
{
    public class CookiesController : Controller
    {
        private const string CookiesName = "Cookies";

        [Inject]
        protected ICurrentCookieLevelProvider CookieLevelProvider { get; set; }

        [HttpGet]
        public ActionResult Cookies()
        {
            int level = this.CookieLevelProvider.GetCurrentCookieLevel();

            var model = new CookiesViewModel()
            {
                IsPerformance = (level == CookieLevel.Essential) || (level == CookieLevel.All),
                IsTracking = (level == CookieLevel.All),
                CookiePolicy = Settings.Cookies.CookiePolicy,
                FunctionalCookies = Settings.Cookies.FunctionalCookieDescription,
                PerformanceCookies = Settings.Cookies.PerformanceCookieDescription,
                TrackingCookies = Settings.Cookies.TrackingCookieDescription
            };

            return PartialView("../Global/Cookies", model);
        }

        [HttpPost]
        public JsonResult AcceptAllCookies()
        {
            try
            {
                CookieLevelProvider.SetCurrentCookieLevel(CookieLevel.All);
                this.ConfirmationCookieAddedForYear();

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(CookiesName, ex.Message, ex);
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Set a specific level of cookies
        /// </summary>
        /// <param name="isPerformance">Cookie level 2 (Essential/0)</param>
        /// <param name="isTracking">Cookie level 3 (All/1000)</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetCookieSettings(bool isPerformance = false, bool isTracking = false)
        {
            try
            {
                int cookieLevel = CookieLevel.System;

                if (isTracking)
                {
                    cookieLevel = CookieLevel.All;
                }
                else if (isPerformance)
                {
                    cookieLevel = CookieLevel.Essential;
                }

                CookieLevelProvider.SetCurrentCookieLevel(cookieLevel);
                this.ConfirmationCookieAddedForYear();

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(CookiesName, ex.Message, ex);
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }

        }

        private void ConfirmationCookieAddedForYear()
        {
            HttpCookie cookie = new HttpCookie(CookiesConstants.CookiesMark)
            {
                Value = Boolean.TrueString.ToLower(),
                Expires = DateTime.Now.AddYears(1)
            };

            Response.Cookies.Add(cookie);
        }
    }
}