using System.Collections.Generic;
using System.Net.Mail;

namespace TGS.Core.Infrastructures.Mail
{
    public record EmailPackage
    {
        public EmailPackage(
            IEnumerable<string> to,
            string subject,
            string content,
            string htmlBodyContent = null,
            IEnumerable<string> cc = null,
            AttachmentData[] attachments = null,
            AttachmentData[] inlineAttachments = null)
        {
            To = new();
            foreach (var s in to)
            {
                To.Add(s);
            }

            Cc = new();
            if (cc != null)
            {
                foreach (var s in cc)
                {
                    Cc.Add(s);
                }
            }

            Subject = subject;
            Content = content;
            HtmlBodyContent = htmlBodyContent;
            Attachments = attachments;
            InlineAttachments = inlineAttachments;
        }

        public MailAddressCollection To { get; }
        public MailAddressCollection Cc { get; }
        public string Subject { get; }
        public string Content { get; }
        public string HtmlBodyContent { get; }
        public AttachmentData[] Attachments { get; }
        public AttachmentData[] InlineAttachments { get; }
    }

    public record AttachmentData
    {
        public string FileName { get; init; }
        public string ContentType { get; init; }
        public byte[] Data { get; init; }
    }
}
