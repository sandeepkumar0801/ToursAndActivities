using Isango.Entities.Enums;
using System;

namespace Isango.Entities.Mailer
{
    public class RuleEngineParameter
    {
        public string Language { get; set; }
        public Guid AffiliateId { get; set; }
        public ActionType MailActionType { get; set; }
    }
}