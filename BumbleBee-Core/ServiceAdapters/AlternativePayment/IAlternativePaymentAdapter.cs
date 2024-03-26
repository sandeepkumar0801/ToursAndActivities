namespace ServiceAdapters.AlternativePayment
{
    public interface IAlternativePaymentAdapter
    {
        Task<Tuple<bool, string>> Create(string authToken, string apiUrl, string transaction, string token);

        Task<object> GetTransaction(string authToken, string apiUrl, string transaction, string token);
    }
}