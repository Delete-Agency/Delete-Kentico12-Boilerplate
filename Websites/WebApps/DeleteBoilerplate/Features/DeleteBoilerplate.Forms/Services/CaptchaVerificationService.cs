using DeleteBoilerplate.Common.Serializers;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Forms.Models;
using RestSharp;

namespace DeleteBoilerplate.Forms.Services
{
    public interface ICaptchaVerificationService
    {
        CaptchaValidationResponse Verify(string captchaResponse);
    }

    public class CaptchaVerificationService : ICaptchaVerificationService
    {
        protected const string CaptchaApiUrl = "https://www.google.com/recaptcha/api/siteverify";

        private string SecretKey { get; }

        public CaptchaVerificationService()
        {
            this.SecretKey = Settings.Integrations.Google.GoogleReCaptchaSecretKey;
        }

        public CaptchaValidationResponse Verify(string captchaResponse)
        {
            var request = this.MakeRestRequest(captchaResponse);
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

        protected RestRequest MakeRestRequest(string captchaResponse)
        {
            var request = new RestRequest(CaptchaApiUrl);

            request.AddParameter("response", captchaResponse);
            request.AddParameter("secret", this.SecretKey);

            return request;
        }
    }
}