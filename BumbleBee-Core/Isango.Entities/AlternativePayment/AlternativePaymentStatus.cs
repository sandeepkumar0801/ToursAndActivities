namespace Isango.Entities.AlternativePayment
{
    public enum AlternativePaymentStatus
    {
        UNDEFINED = 0,
        PENDING = 1,
        FUNDED = 2,
        DECLINED = 3,
        CHARGEBACK = 4
    }
}