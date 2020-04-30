using CMS.EventLog;
using CMS.Helpers;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.WebApp.Constants;
using DeleteBoilerplate.WebApp.Models.Global.Cookies;
using LightInject;
using System;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using CMS.Core;
using CMS.SiteProvider;
using CMS.WebAnalytics;

namespace DeleteBoilerplate.WebApp.Controllers.Global
{
    public class CookiesController : Controller
    {
        private const string CookiesName = "Cookies";

        [Inject]
        protected ICurrentCookieLevelProvider CookieLevelProvider { get; set; }

        protected ICampaignService CampaignService { get; set; }

        public CookiesController()
        {
            this.CampaignService = Service.Resolve<ICampaignService>();
        }

        [HttpGet]
        public ActionResult Cookies()
        {
            var level = this.CookieLevelProvider.GetCurrentCookieLevel();

            var model = new CookiesViewModel
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
        public JsonResult AcceptAllCookies(Uri pageUrl)
        {
            try
            {
                CookieLevelProvider.SetCurrentCookieLevel(CookieLevel.All);
                this.ConfirmationCookieAddedForYear();

                this.AssignCampaignIfExist(pageUrl);

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
        /// <param name="pageUrl">Url of the page where cookie consent was accepted</param>
        /// <param name="isPerformance">Cookie level 2 (Essential/0)</param>
        /// <param name="isTracking">Cookie level 3 (All/1000)</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetCookieLevel(Uri pageUrl, bool isPerformance = false, bool isTracking = false)
        {
            try
            {
                var cookieLevel = CookieLevel.System;

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

                if (cookieLevel >= CookieLevel.Visitor)
                    this.AssignCampaignIfExist(pageUrl);

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(CookiesName, ex.Message, ex);
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }

        }

        private void AssignCampaignIfExist(Uri pageUrl)
        {
            var queryParams = pageUrl.ParseQueryString();

            var campaignCode = queryParams["utm_campaign"];
            if (!string.IsNullOrEmpty(campaignCode))
            {
                var source = queryParams["utm_source"];
                var content = queryParams["utm_content"];
                this.CampaignService.SetCampaign(campaignCode, SiteContext.CurrentSite.SiteName, source, content);
            }
        }

        private void ConfirmationCookieAddedForYear()
        {
            var cookie = new HttpCookie(CookiesConstants.CookiesMark)
            {
                Value = Boolean.TrueString.ToLower(),
                Expires = DateTime.Now.AddYears(1)
            };

            Response.Cookies.Add(cookie);
        }
    }
}