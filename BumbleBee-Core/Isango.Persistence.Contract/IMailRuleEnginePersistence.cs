using Isango.Entities.Mailer;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IMailRuleEnginePersistence
    {
        List<MailHeader> GetMailHeaders(RuleEngineParameter ruleEngineParameter);
    }
}