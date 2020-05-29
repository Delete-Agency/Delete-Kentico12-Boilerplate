﻿using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Infrastructure.Extensions;
using DeleteBoilerplate.Infrastructure.Models.FormComponents.ValidationError;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using LightInject;

namespace DeleteBoilerplate.DynamicRouting.Controllers
{
    public abstract class BaseApiController : Controller
    {
        [Inject]
        protected IMapper Mapper { get; set; }

        protected JsonResult JsonSuccess(object data = null, string message = null, string contentType = null, Encoding contentEncoding = null, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            var jsonData = new JsonData
            {
                Status = JsonStatus.Success,
                Message = message,
                Data = data
            };

            return this.Json(jsonData, contentType, contentEncoding, behavior);
        }

        protected JsonResult JsonValidationError(ModelStateDictionary modelState, string message = null, string contentType = null, Encoding contentEncoding = null, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            this.Response.StatusCode = 422;

            var validationErrors = ValidationErrorModel.Build(this.ModelState);

            var jsonData = new JsonData
            {
                Status = JsonStatus.Error,
                Message = message,
                Data = validationErrors
            };

            return this.Json(jsonData, contentType, contentEncoding, behavior);
        }

        protected JsonResult JsonError(string message = null, object data = null, string contentType = null, Encoding contentEncoding = null, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            this.Response.StatusCode = 500;

            var jsonData = new JsonData
            {
                Status = JsonStatus.Error,
                Message = message,
                Data = data
            };

            return this.Json(jsonData, contentType, contentEncoding, behavior);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                JsonSerializerSettings = JsonExtensions.DefaultJsonSerializerSettings
            };
        }
    }
}