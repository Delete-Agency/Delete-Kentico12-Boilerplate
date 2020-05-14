using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public enum JsonStatus
    {
        Success,
        Error
    }

    public class JsonData
    {
        public JsonStatus Status { get; set; }

        public string Message { get; set; }

         public object Data { get; set; }
    }

    public class JsonNetResult : JsonResult
    {
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
                ? ContentType
                : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            var serializedObject = this.JsonSerializerSettings == null
                ? JsonConvert.SerializeObject(this.Data)
                : JsonConvert.SerializeObject(this.Data, this.JsonSerializerSettings);

            response.Write(serializedObject);
        }
    }
}
