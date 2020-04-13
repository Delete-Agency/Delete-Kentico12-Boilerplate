using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DeleteBoilerplate.Forms.Models
{
    public class CaptchaValidationResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "challenge_ts")]
        public DateTime? ChallengeTime { get; set; }

        [JsonProperty(PropertyName = "hostname")]
        public string Hostname { get; set; }

        [JsonProperty(PropertyName = "error-codes")]
        public List<string> Errors { get; set; }
    }
}