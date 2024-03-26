namespace Isango.Entities.Mailer
{
    public class MailHeader
    {
        public string Subject { get; set; }
        public string From { get; set; }
        public string[] To { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
    }
}