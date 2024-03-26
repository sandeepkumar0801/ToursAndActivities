using Util;

namespace Isango.Mailer.Constants
{
    public sealed class Constant
    {
        public const string SendGridAppKey = "SendGridAppKey";
        public const string ResourceManagerBaseName = "Isango.Mailer.Resources.Common";
        public const string Template = "Templates";
        public const string VoucherTemplateBasePath = "VoucherTemplates";
        public const string MailTemplateBasePath = "MailTemplates";
        public const string QRCodeBasePath = "QRCodes";
        public const string root = "wwwroot";

        public const string Confirmed = "Confirmed";
        public const string ConfirmedFromAllocation = "Confirmed from Allocation";
        public const string OnRequest = "On Request";
        public const string Cancelled = "Cancelled";
        public const string FreeSaleTemplateName = "PDF_FS";
        public const string OnRequestTemplateName = "PDF_OR";
        public const string CancelTemplateName = "PDF_Cancel";
        public const string IsangoLogo = "IsangoLogo_NEW.png";
        public const string IsangoLogoB2B = "Images\\logos\\";
        public const string IsangoLogoB2BUpdate = "/logos/";
        public const string LetSettleImage = "letSettle.png";
        public const string Image = "Images";
        public const string WatermarkImage = "go_png.png";
        public const string PaymentTemplate = "PDF_Payment_";
        public const string PaymentSummaryTemplate = "Invoice";
        public const string Failed = "Failed";
        public const string DE = "de";
        public const string ES = "es";
        public const string FR = "fr";
        public const string ParisTourLogo = "ParisTourLogo.png";
        public const string ActivityDetailUrl = "/Activity/Detail/";
        public const string Germany = "de-De";
        public const string French = "fr-Fr";
        public const string Spanish = "es-Es";
        public const string PostCurrencyLanguages = "PostCurrencyLanguages";
        public const string FileNotFound = "File not found at";
        public const string FooterUrl = "https://www.isango.com/";
        public const string CustomerTemplateBasePath = "CustomerTemplates";

        public const string SupplierConfirmationTemplateName = "SupplierTemplates";
        public const string MailToSupplierForConfirmation = "MailToSupplierForConfirmation";

        public const string SupplierOnRequestTemplateName = "SupplierTemplates";
        public const string MailToSupplierForOnRequest = "MailToSupplierForOnRequest";

        public const string LinkTextForConfirmedProduct = "Click here to Acknowledge.";
        public const string LinkTextForOnRequestProduct = "Click here to Confirm or to Reject.";
        public const string DateFormat = "dd MMM yyyy";
        public const string Tell = "Tell";
        public const string SpecialRequest = "special request";
        public const string ContractComment = "contract comment";
        public const string LeadPassengerDetails = "Lead Passenger details";
        public const string PassengerDetails = "Passenger details";
        public const string SupplierCancellationTemplateName = "SupplierTemplates";
        public const string MailToSupplierForCancellation = "MailToSupplierForCancellation";

        public const string EntertainedByWhomText = "Cancelled by Supplier";
        public const string FailureEmailTemplateName = "CustomerSupportTemplates";
        public const string FailureEmail = "FailureEmail";

        public const string CancelBookingMailTemplateName = "CustomerSupportTemplates";
        public const string CancelBookingMail = "CancelBookingMail";

        public const string DiscountEmailTemplateName = "CustomerSupportTemplates";
        public const string ReminingDiscountEmail = "ReminingDiscountEmail";

        public const string B2bBaseUrl = "B2BBaseURL";
        public const string ReconfirmationPageUrl = "ReconfirmationPageURL";
        public const string PassPhrase = "UnchangePhrase";
        public const string SaltPhrase = "UnchangeValue";
        public const string HashAlgorithm = "SHA1";
        public const string InitVector = "abcdefghijklmnop";
        public const string AdultFirstName = "Any";
        public const string AdultLastName = "Adult";
        public const string ChildFirstName = "Any";
        public const string ChildLastName = "Child";
        public const string MailFrom = "mailfrom";
        public const string MailTo = "mailto";
        public const string MailCc = "mailCC";
        public const string MailBcc = "mailBCC";
        public const string MailSubject = "mailSubject";
        public const string IsangoBaseUrl = "http:/www.isango.com/Activity/Detail/";
        public const string PendingConfirmation = "Pending Confirmation";
        public const string VoucherUrl = "/voucher/book/";
        public const string CancelVoucherUrl = "/voucher/cancel/";
        public const string AmendBookingSubject = "Amended Booking Ref No ";
        public const string BookingRefNo = "Booking Ref No ";
        public const string BookingConfirmationSubject = "Booking Confirmation - ";
        public const string BookingOnRequestSubject = "Booking Request - ";
        public const string AmendMailTemplateName = "AmendEmail_en";
        public const string BookingRefId = "<BookingRefID>";
        public const string Supplier = "<Supplier>";
        public const string SupplierCancellationSubject = "Supplier Cancellation: (";
        public const string AlertMailSubject = "[ALERT]: Your account balance is below the required threshold";
        public const string AlternativeName = "mailFromName";
        public const string CustomerSupportFrom = "CustomerSupportFrom";
        public const string IsangoFinanceEmailId = "IsangoFinanceEmailId";
        public const string FailureMailSubject = "Customer failed booking details";
        public const string CancelBookingMailSubject = "Cancel Booking Detail - ";
        public const string CustomerSupportTo = "CustomerSupportTo";
        public const string BookingManagerCustomerSupportTo = "BookingManagerCustomerSupportTo";
        public const string RemainingDiscountAmountMailSubject = "[ALERT]: Your account balance is below the required threshold";
        public const string TiqetsCustomerMail = "Booking voucher";

        public const string Customer = "<Customer>";
        public const string BookingArchive = "<BookingArchive>";
        public const string CDN = "CDN";
        public const string ImgPath = "/logos/";
        public const string WebAPIBaseUrl = "WebAPIBaseUrl";
        public const string WholeBookingCancelled = "WholeBookingCancelled";
        public const string WholeBookingCancelledTemplateName = "PDF_WholeBookingCancel";
        public const string ErrorSendEmail = "ErrorSendEmail";
        public const string ErrorSendEmailCC = "ErrorSendEmailCC";
        public const string ErrorSendEmailBCC = "ErrorSendEmailBCC";
        public const string ErrorMailEnvironment = "ErrorMailEnvironment";

        public const string BigBusAppBanner = "BigBusAppBanner.jpg";
        public const string BigBusTourLogo = "BigBusTourLogo.png";
        public const string QRCode = "qrCode";

        public const string ApiBookingVoucherDownloadFailed = "[Urgent Action Required]: Booking - {0} || Voucher Download failed!";

        public const string ApiBookingCancellationFailed = "[Urgent Action Required]: Booking - {0} || API Cancellation failed!";


        public const string ApiBookingCancellationSuccess = "[Urgent Action Required]: Booking - {0} || API Cancellation Success!";

        public const string GenerateLinkTemplateName = "GeneratePaymentLink";
        public const string PaymentLink = "PaymentLink_";


        public const string ReceivedLinkTemplateName = "ReceivedPaymentLink";
       public const string ReceivedPayment = "ReceivedPayment_";

        public const string GeneratePaymentLinkMailSubject = "Payment Link";

        public const string ReceivePaymentLinkMailSubject = "Received Payment";

        public const string MailTypeSMTP = "MailTypeSMTP";
        public const string SMTPPort = "SMTPPort";
        public const string SMTPHost = "SMTPHost";

        public const string SMTPFromEmail = "SMTPFromEmail";
        public const string SMTPPassword = "SMTPPassword";
        public const string SMTPPasswordKeyVault = "SMTPPasswordKeyVault";
        public const string SMTPUserName = "SMTPUserName";
        public const string SMTPPasswordHOHO = "SMTPPasswordHOHO";
        public const string SMTPPasswordHOHOKeyVault = "SMTPPasswordHOHOKeyVault";
        public const string SMTPUserNameHOHO = "SMTPUserNameHOHO";
        public const string SMTPPasswordBOAT = "SMTPPasswordBOAT";
        public const string SMTPPasswordBOATKeyVault = "SMTPPasswordBOATKeyVault";
        public const string SMTPUserNameBOAT = "SMTPUserNameBOAT";


        public const string IsangoBarcode = "BAR_CODE";
        public const string IsangoQrCode = "QR_CODE";
        public const string IsangoLink = "LINK";
    }
}