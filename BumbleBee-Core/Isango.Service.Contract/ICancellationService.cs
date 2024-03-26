using System.Collections.Generic;
using System.Threading.Tasks;
using Isango.Entities.Booking;
using Isango.Entities.Cancellation;

namespace Isango.Service.Contract
{
    public interface ICancellationService
    {
        Task<UserCancellationPermission> GetUserCancellationPermissionAsync(string userName);

        Task<CancellationPolicyDetail> GetCancellationPolicyDetailAsync(string bookingRefNo, int bookedOptionId,
            string currencyIsoCode, int spId);

        Task<SupplierCancellationData> GetSupplierCancellationDataAsync(string bookingRefNo);
        Task<List<SupplierCancellationData>> GetSupplierCancellationDataListAsync(string bookingRefNo);
        Task<ConfirmCancellationDetail> CreateCancelBookingIsangoDbAsync(Cancellation cancellation,
            CancellationPolicyDetail cancellationPolicy, int spId, UserCancellationPermission userData);

        void SendCancelBookingMail(CancelBookingMailDetail cancelBookingMailDetail);

        Task<CancellationStatus> GetCancellationStatusAsync(int bookedOptionId);

        void InsertOrUpdateCancellationStatus(CancellationStatus cancellationStatus);

        void UpdateCancelBookingIsangoDbAsync(int transId, PaymentGatewayResponse paymentGatewayResponse, string token);
    }
}