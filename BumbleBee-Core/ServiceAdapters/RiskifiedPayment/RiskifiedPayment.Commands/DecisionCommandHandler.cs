using Isango.Entities.RiskifiedPayment;
using Logger.Contract;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands.Contracts;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities;
using System;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands
{
    public class DecisionCommandHandler : CommandHandlerBase, IDecisionCommandHandler
    {
        public DecisionCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest<T>(T inputContext, object requestExtraData, string token)
        {
            var decisionRequest = new RiskifiedDecisionRequest();
            try
            {
                var decisionRequestData = inputContext as RiskifiedAuthorizationResponse;
                if (decisionRequestData != null)
                {
                    decisionRequest = new RiskifiedDecisionRequest()
                    {
                        Order = new DecisionRequestObject()
                        {
                            Id = decisionRequestData.Order.Id,
                            Decision = new DecisionDetails()
                            {
                                ExternalStatus = decisionRequestData.Order.Status,
                                DecidedAt = DateTime.Now
                            }
                        }
                    };
                }
            }
            catch
            {
                throw;
            }
            return decisionRequest;
        }
    }
}