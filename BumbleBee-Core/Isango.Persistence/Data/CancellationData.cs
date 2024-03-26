using Isango.Entities.Cancellation;
using System.Collections.Generic;
using System.Data;
using Util;

namespace Isango.Persistence.Data
{
    public class CancellationData
    {
        /// <summary>
        /// Map cancellation policy amount data from db
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public CancellationPolicyDetail MapCancellationPolicyData(IDataReader reader)
        {
            var cancellationPolicyAmountData = new CancellationPolicyDetail
            {
              
                BookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDOPTIONID"),
                BookedServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDSERVICEID"),
                ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceID"),
                SellingPrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "SELLING_PRICE"),

                BookedOptionInDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "BOOKEDOPTIONINDATE"),
                CancelDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "CANCELDATE"),

                CancellationChargeDescription = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CANCELLATIONCHARGESDESCRIPTION"),

                UserCancellationCharges =
                    DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "USER_CANCELLATIONCHARGE"),
                UserRefundAmount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "USER_REFUNDAMOUNT"),
                UserCurrencyCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "USER_CURRENCYCODE"),

                ApiTypeId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "APITYPEID"),
                RegPaxId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegPaxID"),
                //Guwid = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "GuWID")
                Guwid="testing"
                //CostPrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "COST_PRICE"),
                //SupplierCancellationCharges = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "SUPPLIER_CANCELLATIONCHARGE"),
                //SupplierCurrencySymbol = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "SUPPLIER_CURRENCYSYMBOL"),
                //SupplierRefundAmount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "SUPPLIER_REFUNDAMOUNT"),
                //SupplierCurrencyCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "SUPPLIER_CURRENCYCODE"),
                //TransactionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "TransID"),
                //AuthorizationCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AuthorizationCode")
            };
            return cancellationPolicyAmountData;
        }

        /// <summary>
        /// Map supplier cancellation data from db
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public SupplierCancellationData MapSupplierCancellationData(IDataReader reader)
        {
            var suppliersData = new SupplierCancellationData
            {
                ApiType = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "APITypeId"),
                BookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookedOptionId"),
                BookedOptionStatusId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "bookedoptionstatusid"),
                BookingReferenceNumber =
                    DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BookingReferenceNumber"),
                CostCurrencyCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CostCurrencyCode"),
                CountryId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CountryID"),
                FHBSupplierShortName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "FHBSuppliershortname"),
                OfficeCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "officeCode"),
                ServiceLongName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceLongName"),
                ServiceOptionName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "serviceoptionname"),
                Status = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "STATUS"),
                SupplierBookingLineNumber =
                    DbPropertyHelper.StringDefaultPropertyFromRow(reader, "supplierbookinglinenumber"),
                SupplierBookingReferenceNumber =
                    DbPropertyHelper.StringDefaultPropertyFromRow(reader, "SupplierBookingReferencenumber"),
                TravelDate = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "TravelDate"),
                CountryName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CountryName")
            };
            return suppliersData;
        }

        /// <summary>
        /// Map user permission data from db
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public UserCancellationPermission MapUserPermissionData(IDataReader reader)
        {
            var userData = new UserCancellationPermission
            {
                UserId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "UserId"),
                IsPermitted = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "HasallPermission")
            };
            return userData;
        }

        /// <summary>
        /// Map cancellation data from db
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ConfirmCancellationDetail MapConfirmedCancellationData(IDataReader reader)
        {
            var confirmedData = new ConfirmCancellationDetail() { TransactionDetail = new List<TransactionDetail>()};
            while (reader.Read())
            {
                var transactionDetail = new TransactionDetail
                {
                    TransId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "transid"),
                    Transflow = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "transflow"),
                    Guwid = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "guwid"),
                    FlowName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "flowname"),
                    Amount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "amount"),
                    Is3D = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Is3D"),
                    CurrencyCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "currencycode"),
                    PaymentGatewayType = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PaymentGatewayType"),
                    PaymentGatewayTypeName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "paymentgatewaytypename"),
                    CaptureGuwid = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "captureguwid"),
                    BookingDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "BookingDate"),
                    AdyenMerchantAccount = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "adyenMerchantAccout")
                };
                confirmedData.TransactionDetail.Add(transactionDetail);
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    confirmedData.SendCancellationEmail = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SendcancellationEmail");
                }
            }
            return confirmedData;
        }

        /// <summary>
        /// Map cancellation status of all steps from db
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public CancellationStatus MapAllCancellationStatus(IDataReader reader)
        {
            var cancellationStatus = new CancellationStatus();
            while (reader.Read())
            {
                cancellationStatus.BookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "bookedoptionid");
                cancellationStatus.IsangoCancelStatus = DbPropertyHelper.Int16PropertyFromRow(reader, "IsangoCancelStatus");
                cancellationStatus.PaymentRefundStatus = DbPropertyHelper.Int16PropertyFromRow(reader, "PaymentRefundStatus");
                cancellationStatus.SupplierCancelStatus = DbPropertyHelper.Int16PropertyFromRow(reader, "SupplierCancelStatus");
            }
            return cancellationStatus;
        }
    }
}