using ServiceAdapters.AlternativePayment.AlternativePayment.Commands.Contracts;

namespace ServiceAdapters.AlternativePayment
{
    public class AlternativePaymentAdapter : IAlternativePaymentAdapter, IAdapter
    {
        #region "Private Members"

        private readonly IAlternativePaymentCommandHandler _paymentCommandHandler;

        #endregion "Private Members"

        #region "Constructor"

        public AlternativePaymentAdapter(IAlternativePaymentCommandHandler paymentCommandHandler)
        {
            _paymentCommandHandler = paymentCommandHandler;
        }

        #endregion "Constructor"

        /// <summary>
        /// Create transaction
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="apiUrl"></param>
        /// <param name="transaction"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> Create(string authToken, string apiUrl, string transaction, string token)
        {
            return await _paymentCommandHandler.CreateTransaction(authToken, apiUrl, transaction, token);
        }

        /// <summary>
        /// Get transaction through transaction id
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="apiUrl"></param>
        /// <param name="transaction"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<object> GetTransaction(string authToken, string apiUrl, string transaction, string token)
        {
            return await _paymentCommandHandler.GetTransaction(authToken, apiUrl, transaction, token);
        }
    }
}