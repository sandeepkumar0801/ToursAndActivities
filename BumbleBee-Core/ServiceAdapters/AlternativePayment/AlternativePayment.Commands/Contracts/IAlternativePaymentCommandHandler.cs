using System;
using System.Threading.Tasks;

namespace ServiceAdapters.AlternativePayment.AlternativePayment.Commands.Contracts
{
    public interface IAlternativePaymentCommandHandler
    {
        Task<Tuple<bool, string>> CreateTransaction(string authToken, string apiUrl, string transaction, string token);

        Task<object> GetTransaction(string authToken, string apiUrl, string transactionId, string token);
    }
}