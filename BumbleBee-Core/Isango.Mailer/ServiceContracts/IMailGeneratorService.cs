using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Mailer;
using Isango.Entities.Mailer.Voucher;
using System.Collections.Generic;

namespace Isango.Mailer.ServiceContracts
{
    public interface IMailGeneratorService
    {
        string GenerateMailBody(TemplateContext templateContext, bool? isAlternativePayment = false, List<Activity> crossSellData = null, bool? isReceive = false, bool? isCancel = false, bool? isORtoConfirm = false);

        string CreateSupplierCancellationEmailContent(BookingDataOthers bookingDataOthers, int serviceId);

        string GenerateSupplierMailBody(BookingDataOthers bookingDataOthers, int serviceId);

        string GenerateAlertMailBody(Affiliate affiliate);

        string GenerateFailureMailBody(List<FailureMailContext> failureMailContextList);

        string GenerateCancelBookingMailBody(List<CancellationMailText> cancellationMailContextList);

        string GenerateDiscountMailBody(string remainingDiscountRows);

        string GenerateCustomerTiqetsTicketMail(string ticketPdfUrl, string bookingRefNo);

        string GenerateGetTicketFailureMail(string bookingRefNo);

        string GeneratePaymentLinkMailBody(string generatedLink, string lang, string tempBookingRefNumber);

        string GeneratePaymentReceivedMailBody(string price, string lang, string tempRef);
    }
}