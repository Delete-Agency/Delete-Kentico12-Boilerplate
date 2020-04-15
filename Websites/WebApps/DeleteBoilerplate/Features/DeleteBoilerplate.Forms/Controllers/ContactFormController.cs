using CMS.EventLog;
using CMS.Helpers;
using CMS.OnlineForms.Types;
using DeleteBoilerplate.Forms.Models;
using System;
using System.Web.Mvc;

namespace DeleteBoilerplate.Forms.Controllers
{
    public class ContactFormController : BaseFormController<ContactFormData>
    {
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
    }
}