using DeleteBoilerplate.Common.Serializers;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Forms.Models;
using RestSharp;
using System.Collections.Specialized;
using System.Linq;

namespace DeleteBoilerplate.Forms.Services
{
    public interface ICaptchaVerificationService
    {
        string CaptchaHeader { get; }

        bool VerifyCaptcha(NameValueCollection requestForm);
    }

    public class CaptchaVerificationService : ICaptchaVerificationService
    {
        protected const string CaptchaApiUrl = "https://www.google.com/recaptcha/api/siteverify";

        public string CaptchaHeader { get; } = "g-recaptcha-response";

        private string SecretKey { get; }

        public CaptchaVerificationService()
        {
            this.SecretKey = Settings.Integrations.Google.GoogleReCaptchaSecretKey;
        }

        public bool VerifyCaptcha(NameValueCollection requestForm)
        {
            if (Settings.Integrations.Google.IsGoogleReCaptchaEnabled == false) 
                return true;

            if (requestForm.AllKeys.Contains(CaptchaHeader) == false)
                return false;

            var captchaUserResponse = requestForm[CaptchaHeader];
            var verificationResult = this.GetVerificationResponse(captchaUserResponse);

            return verificationResult?.Success ?? false;
        }

        private CaptchaValidationResponse GetVerificationResponse(string captchaUserResponse)
        {
            var request = this.MakeRestRequest(captchaUserResponse);
            var client = this.CreateRestClient();

            var response = client.Execute<CaptchaValidationResponse>(request);

            return response?.Data;
        }

        protected RestClient CreateRestClient()
        {
            var client = new RestClient();
            client.AddHandler("application/json", () => new NewtonsoftJsonSerializer());

            return client;
        }

        protected RestRequest MakeRestRequest(string captchaUserResponse)
        {
            var request = new RestRequest(CaptchaApiUrl);

            request.AddParameter("response", captchaUserResponse);
            request.AddParameter("secret", this.SecretKey);

            return request;
        }
    }
}