using Logger.Contract;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands.Contracts;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities;
using System;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands
{
    public class FullRefundCommandHandler : CommandHandlerBase, IFullRefundCommandHandler
    {
        public FullRefundCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest<T>(T bookingReferenceNo, object requestExtraData, string token)
        {
            var fullRefundRequest = new RiskifiedFullRefundRequest();
            try
            {
                fullRefundRequest = new RiskifiedFullRefundRequest()
                {
                    Order = new FullRefundRequest()
                    {
                        Id = bookingReferenceNo.ToString(),
                        CancelReason = "",
                        CancelledAt = DateTime.Now
                    }
                };
            }
            catch
            {
                throw;
            }
            return fullRefundRequest;
        }
    }
}