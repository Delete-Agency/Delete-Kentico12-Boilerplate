using System;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.MacroEngine;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Helpers;
using Kentico.Membership;


namespace DeleteBoilerplate.Domain.Services
{
    public interface IMailService
    {
        bool SendEmail(User user, MacroResolver macro, string templateName);

        bool SendEmail(string email, MacroResolver macro, string templateName);

    }

    public class MailService : IMailService
    {
        public bool SendEmail(User user, MacroResolver macro, string templateName)
        {
            return SendEmail(user.Email, macro, templateName);
        }

        public bool SendEmail(string email, MacroResolver macro, string templateName)
        {
            var emailTemplateInfo = EmailTemplateProvider.GetEmailTemplate(templateName, SiteContext.CurrentSiteID);

            if (emailTemplateInfo is null)
            {
                LogHelper.LogError(this.GetType().Name, "SEND_FAILED", $"No email template found with name.: {templateName}");

                return false;
            }
            var msg = new EmailMessage
            {
                EmailFormat = EmailFormatEnum.Both,
                From = emailTemplateInfo.TemplateFrom,
                ReplyTo = emailTemplateInfo.TemplateReplyTo,
                Recipients = email,
                CcRecipients = emailTemplateInfo.TemplateCc,
                BccRecipients = emailTemplateInfo.TemplateBcc,
                Subject = emailTemplateInfo.TemplateSubject
            };

            PopulateEmailMessage(ref msg, macro);

            try
            {
                EmailSender.SendEmailWithTemplateText(SiteContext.CurrentSiteName, msg, emailTemplateInfo, macro,
                    SettingsKeyInfoProvider.GetBoolValue("CMSEmailQueueEnabled"));
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);

                return false;
            }

            return true;
        }

        private void PopulateEmailMessage(ref EmailMessage message, MacroResolver macro)
        {
            var from = macro.GetNamedSourceData(nameof(EmailMessage.From));
            if (from != null)
            {
                message.From = from as string;
            }
        }
    }
}