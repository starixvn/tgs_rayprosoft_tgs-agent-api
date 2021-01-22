using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Core.Infrastructures.Mail
{
    public interface IEmailManager
    {
        Task SendEmailAsync(Message message);
    }

    public sealed class EmailManager : IEmailManager
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly SmtpClient _smtpClient;

        public EmailManager(EmailConfiguration emailConfig, SmtpClient smtpClient)
        {
            _emailConfig = emailConfig;
            _smtpClient = smtpClient;
        }

        async Task IEmailManager.SendEmailAsync(Message data)
        {
            if (data.To.Count == 0)
            {
                return;
            }

            var message = new MailMessage($"{_emailConfig.FromName} <{_emailConfig.FromMail}>",
                data.To.Select(x => x.Address).JoinAsStringByComma());

            Setup(message, data);
            SetupContent(message, data);
            SetupHtmlContent(message, data);
            SetupCcReceivers(message, data);
            AddAttachments(message, data);

            await _smtpClient.SendMailAsync(message);
            _smtpClient.SendCompleted += (_, _) =>
            {
                _smtpClient.Dispose();
                message.Dispose();
            };
        }

        private static void Setup(MailMessage message, Message data)
        {
            message.Subject = data.Subject;
            message.SubjectEncoding = Encoding.UTF8;
            message.BodyEncoding = Encoding.UTF8;
        }

        private static void SetupContent(MailMessage message, Message data)
        {
            var plainView = AlternateView.CreateAlternateViewFromString(
                data.Content, null, "text/plain");

            message.AlternateViews.Add(plainView);
        }

        private static void SetupHtmlContent(MailMessage message, Message data)
        {
            if (data.HtmlBodyContent == null)
            {
                return;
            }

            var htmlBody = AlternateView.CreateAlternateViewFromString(data.HtmlBodyContent, null, "text/html");
            htmlBody.TransferEncoding = TransferEncoding.QuotedPrintable;
            SetupLinkedResource(htmlBody, data);
            message.AlternateViews.Add(htmlBody);
        }

        private static void SetupLinkedResource(AlternateView htmlBody, Message data)
        {
            if (data.InlineAttachments == null || data.InlineAttachments.Length == 0)
            {
                return;
            }

            foreach (var attachmentData in data.InlineAttachments)
            {
                var ms = new MemoryStream(attachmentData.Data);
                var res = new LinkedResource(ms, attachmentData.ContentType)
                {
                    ContentId = attachmentData.FileName
                };

                htmlBody.LinkedResources.Add(res);
            }
        }

        private static void SetupCcReceivers(MailMessage message, Message data)
        {
            if (data.Cc.Count > 0)
            {
                message.CC.Add(data.Cc.Select(x => x.Address).JoinAsStringByComma());
            }
        }

        private static void AddAttachments(MailMessage message, Message data)
        {
            if (data.Attachments == null || data.Attachments.Length == 0)
            {
                return;
            }

            foreach (var attachmentData in data.Attachments)
            {
                var ms = new MemoryStream(attachmentData.Data);
                // processing in MailMessage will dispose this stream
                var attachment = new Attachment(ms, attachmentData.FileName);
                message.Attachments.Add(attachment);
            }
        }
    }
}