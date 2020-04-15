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
                var form = Mapper.Map<ContactItem>(formData);
                form.SubmitChanges(false);

                string message = ResHelper.GetString("DeleteBoilerplate.Forms.Contact.Success");
                return Json(new { Result = true, Message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Contact form submit", ex.ToString(), null);
                return Json(new { Result = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}