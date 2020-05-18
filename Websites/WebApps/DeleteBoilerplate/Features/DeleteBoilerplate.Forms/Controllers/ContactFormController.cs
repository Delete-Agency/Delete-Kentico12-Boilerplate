using CMS.EmailEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.OnlineForms.Types;
using DeleteBoilerplate.Common.Helpers;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Domain.Services;
using DeleteBoilerplate.Forms.Models;
using LightInject;
using System;
using System.Web.Mvc;

namespace DeleteBoilerplate.Forms.Controllers
{
    public class ContactFormController : BaseFormController<ContactFormData>
    {
        [Inject]
        protected IMailService MailService { get; set; }

        [HttpPost]
        public ActionResult Submit(ContactFormData formData)
        {
            return this.ProcessForm(formData);
        }

        protected override ActionResult ProcessFormInternal(ContactFormData formData)
        {
            try
            {
                this.SaveFormData<ContactItem>(formData);

                if (Settings.Notifications.Forms.IsSendEmailInContactForm)
                    this.SendEmail(formData);

                return JsonSuccess(message: ResHelper.GetString("DeleteBoilerplate.Forms.Contact.Success"));
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);

                return JsonError(ResHelper.GetString("DeleteBoilerplate.Forms.Contact.Error"));
            }
        }

        private void SendEmail(ContactFormData formData)
        {
            var macroResolver = this.BuildMacroResolver(formData);

            this.MailService.SendEmail(formData.Email, macroResolver, Constants.EmailTemplates.ContactForUser);
        }

        private MacroResolver BuildMacroResolver(ContactFormData formData)
        {
            var macroResolver = MacroResolver.GetInstance();

            macroResolver.SetNamedSourceData("Title", ResHelper.GetString("Title"));
            macroResolver.SetNamedSourceData("Description", ResHelper.GetString("Description"));

            macroResolver.SetNamedSourceData(nameof(EmailMessage.From), formData.Email);
            macroResolver.SetNamedSourceData("UserEmail", formData.Email);
            macroResolver.SetNamedSourceData("UserPhone", formData.Telephone);
            macroResolver.SetNamedSourceData("UserMessage", formData.Message);
            macroResolver.SetNamedSourceData("UserFirstName", formData.FirstName);
            macroResolver.SetNamedSourceData("UserFullName", $"{formData.FirstName} {formData.LastName}");
            return macroResolver;
        }
    }
}