using Microsoft.Extensions.Options;
using Span.Notification.SMTP.Core;
using Span.Notification.SMTP.Service.IContract;
using System.Net.Mail;
using System.Net;

namespace Span.Notification.SMTP.Service
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;

        public EmailService(IOptions<AppSettings> settings)
        {
            _appSettings = settings.Value;

        }
        public string Send(EmailDetail emailDetail, out bool isSuccess)
        {
            try
            {           
                var mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(_appSettings.SMTPMailFromAddress??string.Empty);
                mailMessage.Subject = emailDetail.Subject;

                if (emailDetail.To != null)
                    emailDetail.To.ForEach(emailAddress => mailMessage.To.Add(new MailAddress(emailAddress)));

                mailMessage.Body = emailDetail.Body;
                mailMessage.IsBodyHtml = _appSettings.IsSMTPHtmBody;

                if (emailDetail.CC != null)
                    emailDetail.CC.ForEach(x => mailMessage.CC.Add(x));

                if (emailDetail.BCC != null)
                    emailDetail.BCC.ForEach(x => mailMessage.Bcc.Add(x));

                if (emailDetail.Attachment != null)
                    mailMessage.Attachments.Add(emailDetail.Attachment);

                using (SmtpClient smtpClient = new SmtpClient(_appSettings.SMTPHost, _appSettings.SMTPPort))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_appSettings.SMTPUserName, _appSettings.SMTPPassword);
                    smtpClient.EnableSsl = _appSettings.SMTPEnableSSL;
                    smtpClient.Send(mailMessage);
                }

                isSuccess = true;
                return "Success";
            }
            catch (Exception ex)
            {

                isSuccess = false;
                return ex.Message;
            }

        }
    }
}
