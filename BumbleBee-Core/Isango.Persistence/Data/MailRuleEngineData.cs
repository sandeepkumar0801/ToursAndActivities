using Isango.Entities.Mailer;
using System.Data;
using Util;

namespace Isango.Persistence.Data
{
    public class MailRuleEngineData
    {
        public MailHeader GetMailHeaders(IDataReader reader)
        {
            var header = new MailHeader
            {
                From = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MailFrom"),
                Subject = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MailSubject")
            };

            var to = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MailTo");
            if (!string.IsNullOrWhiteSpace(to)) { header.To = to.Split(','); }

            var cc = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MailCC");
            if (!string.IsNullOrWhiteSpace(cc)) { header.CC = cc.Split(','); }

            var bcc = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MailBCC");
            if (!string.IsNullOrWhiteSpace(bcc)) { header.BCC = bcc.Split(','); }

            return header;
        }
    }
}