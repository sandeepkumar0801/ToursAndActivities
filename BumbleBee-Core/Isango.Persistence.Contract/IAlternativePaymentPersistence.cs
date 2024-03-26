namespace Isango.Persistence.Contract
{
    public interface IAlternativePaymentPersistence
    {
        bool CompleteIsangoBookingAfterTransaction(string bookingReferenceNo, string transactionId, string token);

        string GetBookingRefByTransId(string transId);

        bool UpdateSofortChargeBack(string transId, string status);
    }
}