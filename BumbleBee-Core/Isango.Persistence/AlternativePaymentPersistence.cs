using Isango.Entities;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Data;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class AlternativePaymentPersistence : PersistenceBase, IAlternativePaymentPersistence
    {
        private readonly ILogger _log;
        public AlternativePaymentPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Method to complete the booking after successful transaction
        /// </summary>
        /// <param name="bookingReferenceNo"></param>
        /// <param name="transactionId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool CompleteIsangoBookingAfterTransaction(string bookingReferenceNo, string transactionId, string token)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.CompleteIsangoBookingAfterTransactionSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingRefNoParam, DbType.String, bookingReferenceNo);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Guid, DbType.String, transactionId);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AlternativePaymentPersistence",
                    MethodName = "CompleteIsangoBookingAfterTransaction",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Transaction by transaction id
        /// </summary>
        /// <param name="transId"></param>
        /// <returns></returns>
        public string GetBookingRefByTransId(string transId)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingRefByTransIdSp))
                {
                    var bookingRefNo = string.Empty;
                    IsangoDataBaseLive.AddInParameter(command, Constant.TransactionId, DbType.String, transId);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            bookingRefNo = DbPropertyHelper.StringPropertyFromRow(reader, Constant.BookingRefNo);
                        }
                    }
                    return bookingRefNo;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AlternativePaymentPersistence",
                    MethodName = "GetBookingRefByTransId",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Update status for sofort transaction
        /// </summary>
        /// <param name="transId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateSofortChargeBack(string transId, string status)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.UpdateSofortChargeBackSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.TransId, DbType.String, transId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.SofortStatus, DbType.String, status);
                    var isUpdated = IsangoDataBaseLive.ExecuteNonQuery(command);
                    return isUpdated != -1;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AlternativePaymentPersistence",
                    MethodName = "UpdateSofortChargeBack",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}