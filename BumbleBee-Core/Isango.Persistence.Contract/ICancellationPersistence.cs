using Isango.Entities.Booking;
using Isango.Entities.Booking.ConfirmBooking;
using Isango.Entities.Cancellation;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface ICancellationPersistence
    {
        CancellationPolicyDetail GetCancellationPolicyDetail(string bookingRefNo, int bookedOptionId,
            string currencyIsoCode, int spId);

        SupplierCancellationData GetSupplierCancellationData(string bookingRefNo);
        List<SupplierCancellationData> GetSupplierCancellationDataList(string bookingRefNo);
        UserCancellationPermission GetUserPermissionForCancellation(string userName);

        ConfirmCancellationDetail CreateCancelBooking(Cancellation cancellation, string bookingRefNo,
            string cancelledById, int cancelledByUser);

        CancellationStatus GetCancellationStatus(int bookedOptionId);

        void InsertOrUpdateCancellationStatus(CancellationStatus cancellationStatus);

        void UpdateCancelBooking(int transId, PaymentGatewayResponse paymentGatewayResponse);

        List<BookedOptionMailData> CheckToSendmailToCustomer(int bookedOptionId);
    }
}