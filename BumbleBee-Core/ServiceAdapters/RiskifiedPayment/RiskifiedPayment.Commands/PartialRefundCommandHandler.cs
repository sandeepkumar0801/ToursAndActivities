using Isango.Entities.Booking;
using Logger.Contract;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands.Contracts;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands
{
    public class PartialRefundCommandHandler : CommandHandlerBase, IPartialRefundCommandHandler
    {
        public PartialRefundCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object CreateInputRequest<T>(T inputContext, object requestExtraData, string token)
        {
            var partialRefundRequest = new RiskifiedPartialRefundRequest();
            try
            {
                var booking = inputContext as Booking;
                if (booking != null)
                {
                    List<RefundDetail> refundDetails = new List<RefundDetail>();
                    if (booking?.SelectedProducts?.Count > 0)
                    {
                        foreach (var selectedProduct in booking.SelectedProducts)
                        {
                            var refundDetail = new RefundDetail()
                            {
                                RefundId = "",
                                RefundedAt = DateTime.Now,
                                ServiceId = selectedProduct.Id,
                                Amount = (float)selectedProduct.Price,
                                Currency = selectedProduct.SupplierCurrency,
                                Reason = ""
                            };
                            refundDetails.Add(refundDetail);
                        }
                    }
                    partialRefundRequest = new RiskifiedPartialRefundRequest()
                    {
                        Order = new PartialRefundRequestObject()
                        {
                            Id = booking.ReferenceNumber,
                            Refunds = refundDetails
                        }
                    };
                }
            }
            catch
            {
                throw;
            }

            return partialRefundRequest;
        }
    }
}