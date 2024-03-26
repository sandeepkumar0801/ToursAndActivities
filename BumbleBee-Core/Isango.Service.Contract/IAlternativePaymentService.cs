using Isango.Entities.AlternativePayment;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IAlternativePaymentService
    {
        Task<Transaction> GetAlternativeTransaction(string transactionId, string authToken, string token, string baseUrl = null);

        Task<bool> CompleteTransactionAfterBookingAsync(string bookingReferenceNo, string transactionId, string token);

        Task<string> GetBookingRefByTransIdAsync(string transId);

        //called from web hook
        Task<bool> UpdateSofortChargeBackAsync(string transId, string status);
    }
}