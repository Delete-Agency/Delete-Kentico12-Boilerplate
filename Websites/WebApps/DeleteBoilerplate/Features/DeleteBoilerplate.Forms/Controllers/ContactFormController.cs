using CMS.EmailEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.OnlineForms.Types;
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

                string successMessage = ResHelper.GetString("DeleteBoilerplate.Forms.Contact.Success");
                return Json(new { Result = true, Message = successMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("ContactForm Submit", ex.ToString(), null);

                string errorMessage = ResHelper.GetString("DeleteBoilerplate.Forms.Contact.Error");
                return Json(new { Result = false, errorMessage }, JsonRequestBehavior.AllowGet);
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