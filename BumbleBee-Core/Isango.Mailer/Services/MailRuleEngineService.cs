using Isango.Entities;
using Isango.Entities.Mailer;
using Isango.Mailer.Constants;
using Isango.Mailer.ServiceContracts;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Collections.Generic;
using Util;

namespace Isango.Mailer.Services
{
	public class MailRuleEngineService : IMailRuleEngineService
	{
		private readonly IMailRuleEnginePersistence _mailRuleEnginePersistence;
		private readonly ILogger _log;

		public MailRuleEngineService(IMailRuleEnginePersistence mailRuleEnginePersistence, ILogger log)
		{
			_mailRuleEnginePersistence = mailRuleEnginePersistence;
			_log = log;
		}

		/// <summary>
		/// Method to get Mail headers.
		/// </summary>
		/// <param name="templateContext"></param>
		/// <returns></returns>
		public List<MailHeader> GetMailHeaders(TemplateContext templateContext)
		{
			try
			{
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
				if (templateContext == null) { return null; }
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

				List<MailHeader> mailHeaders;
				var affiliateId = templateContext.AffiliateId.ToString().ToUpperInvariant();
				if (affiliateId == "4D22F371-139C-4C37-B605-69696D1C9C93" || affiliateId == "B76BCC30-11D1-42DB-9D75-A2C3E38F91D8" || affiliateId == "F9AD9891-12DB-424A-BF78-609F86BBD70E")
				{
					//Set rule engine parameters
					var ruleEngineParameter = new RuleEngineParameter
					{
						AffiliateId = templateContext.AffiliateId,
						Language = templateContext.Language,
						MailActionType = templateContext.MailActionType
					};

					mailHeaders = _mailRuleEnginePersistence.GetMailHeaders(ruleEngineParameter);
				}
				else if (templateContext.MailContextList?.Count > 0)
				{
					mailHeaders = templateContext.MailContextList;
				}
				else
				{
					mailHeaders = GetMailHeadersFromConfig();
				}

				return mailHeaders;
			}
			catch (Exception ex)
			{
				var isangoErrorEntity = new IsangoErrorEntity
				{
					ClassName = "MailRuleEngineService",
					MethodName = "GetMailHeaders",
					Params = $"{SerializeDeSerializeHelper.Serialize(templateContext)}"
				};
                _log.Error(isangoErrorEntity, ex);
				throw;
			}
		}

		#region "Private Mthods"

		/// <summary>
		/// Creates mail headers from values stored in config file.
		/// </summary>
		/// <returns></returns>
		private List<MailHeader> GetMailHeadersFromConfig()
		{
			var mailContextList = new List<MailHeader>
			{
				new MailHeader
				{
					From = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MailFrom),
					To = new[] {ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MailTo)},
					CC = new[] {ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MailCc)},
					BCC = new[] {ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MailBcc)},
					Subject = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MailSubject)
				}
			};

			return mailContextList;
		}

		#endregion "Private Mthods"
	}
}