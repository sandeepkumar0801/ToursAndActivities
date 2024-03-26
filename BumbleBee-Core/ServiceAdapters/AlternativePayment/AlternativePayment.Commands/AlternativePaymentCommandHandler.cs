using Logger.Contract;
using ServiceAdapters.AlternativePayment.AlternativePayment.Commands.Contracts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.AlternativePayment.AlternativePayment.Commands
{
    public class AlternativePaymentCommandHandler : IAlternativePaymentCommandHandler
    {
        private readonly ILogger _log;

        /// <summary>
        /// constructor
        /// </summary>
        public AlternativePaymentCommandHandler(ILogger log)
        {
            _log = log;
        }

        /// <summary>
        /// Creating transaction
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="apiUrl"></param>
        /// <param name="transaction"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> CreateTransaction(string authToken, string apiUrl, string transaction, string token)
        {
            var watch = Stopwatch.StartNew();

            var client = new AsyncClient { ServiceURL = apiUrl };
            var response = await client.SendPostRequest(authToken, transaction);
            watch.Stop();
            _log.WriteTimer("CreateTransaction", token, "AlternativePayment", watch.Elapsed.ToString());
            _log.Write(transaction, response.Item2, "CreateTransaction", token, "AlternativePayment");
            return response;
        }

        /// <summary>
        /// Get transactions through transaction id
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="apiUrl"></param>
        /// <param name="transactionId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<object> GetTransaction(string authToken, string apiUrl, string transactionId, string token)
        {
            var watch = Stopwatch.StartNew();
            var client = new AsyncClient { ServiceURL = apiUrl };
            var response = await client.SendGetRequest(authToken, transactionId);
            watch.Stop();
            _log.WriteTimer("GetTransaction", token, "AlternativePayment", watch.Elapsed.ToString());
            _log.Write(transactionId, response, "GetTransaction", token, "AlternativePayment");
            return response;
        }
    }
}