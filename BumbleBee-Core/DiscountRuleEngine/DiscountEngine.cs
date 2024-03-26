using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Model;
using Isango.Entities;
using Logger.Contract;
using System;
using Util;

namespace DiscountRuleEngine
{
	public class DiscountEngine : IDiscountEngine
	{
		private readonly IDiscountProcessor _processor;
		private readonly ILogger _log;
		public DiscountEngine(IDiscountProcessor processor, ILogger log)
		{
			_processor = processor;
			_log = log;
		}

		public DiscountCart Process(DiscountModel discountModel)
		{
			try
			{
				if (discountModel == null) return null;
				return _processor.Process(discountModel);
			}
			catch (Exception ex)
			{
				var isangoErrorEntity = new IsangoErrorEntity
				{
					ClassName = "DiscountEngine",
					MethodName = "Process",
					AffiliateId = discountModel.AffiliateId,
					Params = $"{SerializeDeSerializeHelper.Serialize(discountModel)}"
				};
                _log.Error(isangoErrorEntity, ex);
				throw;
			}
		}
	}
}