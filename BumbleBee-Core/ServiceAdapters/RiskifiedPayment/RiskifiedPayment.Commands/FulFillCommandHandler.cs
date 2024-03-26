using Isango.Entities.Booking;
using Logger.Contract;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands.Contracts;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands
{
    public class FulFillCommandHandler : CommandHandlerBase, IFulFillCommandHandler
    {
        public FulFillCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest<T>(T inputContext, object requestExtraData, string token)
        {
            var fulFillRequest = new RiskifiedFulFillRequest();
            try
            {
                var fulFillRequestData = inputContext as Booking;
                if (fulFillRequestData != null)
                {
                    fulFillRequest = new RiskifiedFulFillRequest()
                    {
                        Order = new RiskifiedFulFillRequestObject()
                        {
                            Id = fulFillRequestData.Id,
                            Fulfillments = new List<FulfillmentDetails>()
                        {
                            new FulfillmentDetails()
                            {
                                FulfillmentId = "",
                                CreatedAt = DateTime.Now,
                                Status = ""
                            }
                        }
                        }
                    };
                }
            }
            catch
            {
                throw;
            }
            return fulFillRequest;
        }
    }
}