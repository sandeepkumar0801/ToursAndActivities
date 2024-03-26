using Isango.Entities.Booking;
using Isango.Entities.Mailer;
using System.Collections.Generic;

namespace Isango.Mailer.ServiceContracts
{
	public interface ITemplateContextService
	{
		TemplateContext GetEmailTemplateContext(Booking bookingDetail, Dictionary<string, string> numbers,bool? isReceive = false);

		TemplateContext GetCancelledEmailTemplateContext(Booking bookingDetail, Dictionary<string, string> numbers);
	}
}
