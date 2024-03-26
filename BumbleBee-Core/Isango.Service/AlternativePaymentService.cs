using Isango.Entities;
using Isango.Entities.AlternativePayment;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.AlternativePayment;
using Util;

namespace Isango.Service
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class AlternativePaymentService : IAlternativePaymentService
    {
        private readonly ILogger _log;
        private readonly IAlternativePaymentAdapter _alternativePaymentAdapter;

        private readonly IAlternativePaymentPersistence _alternativePaymentPersistence;

        public AlternativePaymentService(ILogger log, IAlternativePaymentAdapter alternativePaymentAdapter,
            IAlternativePaymentPersistence alternativePaymentPersistence)
        {
            _log = log;
            _alternativePaymentAdapter = alternativePaymentAdapter;
            _alternativePaymentPersistence = alternativePaymentPersistence;
        }

        public async Task<Transaction> GetAlternativeTransaction(string transactionId, string authToken, string token, string baseUrl = null)
        {
            try
            {
                var result = await _alternativePaymentAdapter.GetTransaction(authToken, baseUrl, transactionId, token);
                var transactionResult = SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<Transaction>(result.ToString());
                return transactionResult;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AlternativePaymentService",
                    MethodName = "GetAlternativeTransaction",
                    Token = token,
                    Params = $"{transactionId},{SerializeDeSerializeHelper.Serialize(baseUrl)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<bool> CompleteTransactionAfterBookingAsync(string bookingReferenceNo, string transactionId, string token)
        {
            try
            {
                var result = _alternativePaymentPersistence.CompleteIsangoBookingAfterTransaction(bookingReferenceNo, transactionId, token);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AlternativePaymentService",
                    MethodName = "CompleteTransactionAfterBooking",
                    Token = token,
                    Params = $"{bookingReferenceNo},{transactionId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<string> GetBookingRefByTransIdAsync(string transId)
        {
            try
            {
                return await Task.FromResult(_alternativePaymentPersistence.GetBookingRefByTransId(transId));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AlternativePaymentService",
                    MethodName = "GetBookingRefByTransId",
                    Params = $"{transId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public async Task<bool> UpdateSofortChargeBackAsync(string transId, string status)
        {
            try
            {
                return await Task.FromResult(_alternativePaymentPersistence.UpdateSofortChargeBack(transId, status));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AlternativePaymentService",
                    MethodName = "UpdateSofortChargeBack",
                    Params = $"{transId},{status}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}