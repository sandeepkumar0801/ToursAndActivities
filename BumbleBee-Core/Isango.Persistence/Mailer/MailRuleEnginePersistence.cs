using Isango.Entities;
using Isango.Entities.Mailer;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.Mailer
{
    public class MailRuleEnginePersistence : PersistenceBase, IMailRuleEnginePersistence
    {
        private readonly ILogger _log;
        public MailRuleEnginePersistence(ILogger log)
        {
            _log = log;
        }
        public List<MailHeader> GetMailHeaders(RuleEngineParameter ruleEngineParameter)
        {

            var mailHeaders = new List<MailHeader>();
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetKayakoMailHeaderSp))
                {
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.AffiliateIdForRegion, DbType.String, ruleEngineParameter.AffiliateId.ToString());
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.LanguageCodeForLoadRegionMetaData, DbType.String, ruleEngineParameter.Language);
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.KayakoActionTypeId, DbType.String, (int)ruleEngineParameter.MailActionType);
                    dbCommand.CommandType = CommandType.StoredProcedure;

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var mailRuleEngineData = new MailRuleEngineData();
                        while (reader.Read())
                        {
                            mailHeaders.Add(mailRuleEngineData.GetMailHeaders(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailRuleEnginePersistence",
                    MethodName = "GetMailHeaders",
                    Params = $"{SerializeDeSerializeHelper.Serialize(ruleEngineParameter)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return mailHeaders;
        }
    }
}