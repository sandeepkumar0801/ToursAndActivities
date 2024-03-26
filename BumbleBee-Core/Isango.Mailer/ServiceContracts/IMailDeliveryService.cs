using Isango.Entities.Mailer;
using System.Collections.Generic;

namespace Isango.Mailer.ServiceContracts
{
	public interface IMailDeliveryService
	{
		void SendMail(MailContext sendGridMessage, List<System.Net.Mail.Attachment> attachments = null);
	}
}