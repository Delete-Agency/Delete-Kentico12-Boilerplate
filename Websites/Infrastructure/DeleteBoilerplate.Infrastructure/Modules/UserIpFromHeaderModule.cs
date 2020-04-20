using System;
using System.Net;
using System.Web;
using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Infrastructure.Modules;

[assembly: RegisterModule(typeof(UserIpFromHeaderModule))]
namespace DeleteBoilerplate.Infrastructure.Modules
{
    public class UserIpFromHeaderModule : Module
    {
        // Module class constructor, the system registers the module under the name "IpFromHeader"
        public UserIpFromHeaderModule() : base("DeleteBoilerplate.Modules.IpFromHeader")
        {
        }

        // Contains initialization code that is executed when the application starts
        protected override void OnInit()
        {
            base.OnInit();
            // Assigns custom handler to begin request event
            RequestEvents.Begin.Execute += this.GetRealClientIpAddress;
        }

        void GetRealClientIpAddress(object sender, EventArgs e)
        {
            if ((HttpContext.Current != null) && (HttpContext.Current.Request != null))
            {
                var headers = HttpContext.Current.Request.Headers;

                // Check that header is filled in Settings
                if (Settings.System.IpHeaderKey.IsNullOrWhiteSpace())
                    return;

                // Look for the special header variable that the firewall or load balancer passes
                var headerValue = headers.Get(Settings.System.IpHeaderKey);

                if (!string.IsNullOrWhiteSpace(headerValue) && !string.Equals(headerValue, "::1", StringComparison.OrdinalIgnoreCase))
                {
                    headerValue = this.GetIpAddress(headerValue);

                    RequestContext.UserHostAddress = headerValue;
                }
            }
        }

        public string GetIpAddress(string headerValue)
        {
            if (headerValue.Contains(","))
                headerValue = headerValue.Split(',')[0].Trim();

            headerValue = this.StripPortFromIpString(headerValue);

            return headerValue;
        }

        public string StripPortFromIpString(string ipString)
        {
            var splitList = ipString.Split(':');

            if (splitList.Length > 2)
            {
                var parseResult = IPAddress.TryParse(ipString, out var ipAddress);

                if (parseResult)
                    ipString = ipAddress.ToString();
            }
            else if (splitList.Length == 2)
            {
                ipString = splitList[0];
            }

            return ipString.Trim();
        }
    }
}