using Isango.Entities.Mailer;
using System.Collections.Generic;

namespace Isango.Mailer.ServiceContracts
{
	public interface IMailRuleEngineService
	{
		List<MailHeader> GetMailHeaders(TemplateContext ruleEngineParameter);
	}
}
