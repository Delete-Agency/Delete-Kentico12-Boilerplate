using System;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.MacroEngine;
using CMS.SiteProvider;
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
                EventLogProvider.LogEvent(EventType.ERROR, "Email Sender", "Send Failed", eventDescription:
                    $"No email template found with name.: {templateName}");
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
            catch (Exception e)
            {
                EventLogProvider.LogException("Email Sender", "Send failed", e);
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