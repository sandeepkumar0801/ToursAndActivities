using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Mailer;
using Isango.Entities.Mailer.Voucher;
using System;
using System.Collections.Generic;

namespace Isango.Mailer.Contract
{
    public interface IMailer
    {
        void SendMail(TemplateContext templateContext, List<System.Net.Mail.Attachment> attachments = null, bool? isAlternativePayment = false, List<Activity> crossSellData = null, bool? isReceive = false, bool? isCancel = false, bool? isORtoConfirm = false);

        void SendCancellationMailToSupplier(TemplateContext templateContext,
            BookingDataOthers bookingDataOthers, OthersBookedProductDetail bookedProductDetail);

        void SendSupplierMail(TemplateContext templateContext, BookingDataOthers bookingDataOthers, int serviceId);

        void SendAlertMail(Affiliate affiliate);

        void SendRemainingDiscountAmountMail(string remainingRows);

        void SendFailureMail(List<FailureMailContext> failureMailContextList);

        void SendCancelBookingMail(List<CancellationMailText> cancellationMailContextList);

        void SendErrorMail(List<Tuple<string, string>> data);

        void SendTiqetsBookingTicket(string ticketPdfPath, string bookingRefNo, string customerEmail);

        void SendGetTicketFailureMail(string bookingRefNo);

        void SendPDFErrorMail(string bookingRefNo);

        void SendAdyenWebhookErrorMail(string bookingRefNo, string status, string pspReference, string reason);

        void SendVoucherDownloadFailureMail(string bookingRefNo, string apiType = null, string request = null, string response = null);

        void SendAdyenGenerateLinkMail(string customerEmail, string generatedLink, string lang, string tempBookingRefNumber);

        void SendAdyenReceivedLinkMail(string customerEmail, string price, string lang, string tempRef);


        void SendCancellationFailureMail(string bookingRefNo, string apiType = null, string request = null, string response = null);
        void SendCancellationSuccessMail(string bookingRefNo, string apiType = null, string request = null, string response = null);
    }
}