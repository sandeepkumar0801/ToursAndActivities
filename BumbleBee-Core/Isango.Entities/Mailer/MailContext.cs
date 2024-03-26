using SendGrid.Helpers.Mail;

namespace Isango.Entities.Mailer
{
    public class MailContext
    {
        public string Subject { get; set; }
        public EmailAddress From { get; set; }
        public string[] To { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public string HtmlContent { get; set; }
        public Attachment Attachment { get; set; }
    }
}