namespace Isango.Entities.Enums
{
    public enum ActionType
    {
        Undefined = 0,

        Registration = 1,

        CustomerVoucherOR = 2,

        SupplierVoucherOR = 3,

        CustomerVoucherFS = 4,

        SupplierVoucherFS = 5,

        /// <summary>
        /// Booking confirmation Email to Customer
        /// </summary>
        CustomerVoucherORConfirmation = 6,

        /// <summary>
        /// Booking confirmation Email to supplier
        /// </summary>
        KayakoVoucherORConfirmation = 7,

        CoustomerVoucherCancellation = 8,

        SupplierVoucherCancellation = 9,

        BookingFailureCodeException = 10,

        BookingFailureFeedbackForm = 11,

        BookingPartialPayment = 12, // for LOT only

        VoucherPDFAttachment = 13,

        CancelPDFAttachment = 14,

        EMIPaymentConfirmation = 15,

        EMIPaymentNotReceived = 16,

        EMIPaymentReminder = 17,

        AmendmentPaymentReceive = 18,

        EMISecurePayment = 19,

        GiftVoucher = 21,

        EnquiryEmail = 22,

        ResetPassword = 23
    }
}