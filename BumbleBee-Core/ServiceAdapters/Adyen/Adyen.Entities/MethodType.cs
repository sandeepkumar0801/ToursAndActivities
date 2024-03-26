namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public enum MethodType
    {
        PaymentMethods=1,
        EnrollmentCheck = 2,
        ThreeDSVerify = 3,
        CaptureCard = 4,
        RefundCard = 5,
        CancelCard = 6,
        PaymentLinks=7
    }
}