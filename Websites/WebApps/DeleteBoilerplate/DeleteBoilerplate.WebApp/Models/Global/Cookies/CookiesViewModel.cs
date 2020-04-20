namespace DeleteBoilerplate.WebApp.Models.Global.Cookies
{
    public class CookiesViewModel
    {
        public bool IsFunctional { get; } = true;

        public bool IsPerformance { get; set; }

        public bool IsTracking { get; set; }

        public string CookiePolicy { get; set; }

        public string FunctionalCookies { get; set; }

        public string PerformanceCookies { get; set; }

        public string TrackingCookies { get; set; }
    }
}