using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.TourCMS;
using Isango.Entities.TourCMSCriteria;
using ServiceAdapters.TourCMS.TourCMS.Entities.CancelBookingResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.CheckAvailabilityResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.DatesnDealsResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.DeleteBookingResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.NewBooking;
using ServiceAdapters.TourCMS.TourCMS.Entities.Redemption;

namespace ServiceAdapters.TourCMS
{
    public interface ITourCMSAdapter
    {
        ChannelListResponse GetChannelData(string token, int channelId);
        ChannelShowResponse GetChannelShowData(string token, int channelId);

        TourListResponse GetTourData(string token, int channelId);
        TourShowResponse GetTourShowData(string token, int channelId, int tourId);
        DatesnDealsResponse GetCalendarDatafromAPI(TourCMSCriteria criteria, string tokenId);
        List<Activity> GetOptionsForTourCMSActivity(TourCMSCriteria criteria, string token);
        CheckAvailabilityResponse GetAvailablityfromAPI(TourCMSCriteria criteria, string tokenId);
        Isango.Entities.Activities.Activity ActivityDetails(TourCMSCriteria criteria, string tokenId);


        List<SelectedProduct> CreateReservation(
            Booking booking, string token,
            out string request, out string response);

        List<SelectedProduct> CommitBooking(
           List<SelectedProduct> selectedProducts, string token,
           out string request, out string response);

        CancelBookingResponse CancelBooking(int bookingId, string prefixServiceCode, string token,
        out string apiRequest, out string apiResponse);

        NewBookingResponse CreateReservationOnly(
          SelectedProduct selectedProduct, string languageCode,
          string voucherEmailAddress, string voucherPhoneNumber,
          string referenceNumber, string token,
          out string request, out string response);

        SelectedProduct CommitBookingSingle(
           SelectedProduct selectedProduct, string token,
           out string request, out string response);

        SelectedProduct CreateReservationSingle(
          SelectedProduct selectedProduct, string languageCode,
          string voucherEmailAddress, string voucherPhoneNumber,
          string referenceNumber, string token,
          out string request, out string response
         );

        DeleteBookingResponse DeleteBooking(int bookingId, string prefixServiceCode, string token,
          out string apiRequest, out string apiResponse);

        RedemptionResponse RedemptionBookingData(TourCMSRedemptionCriteria criteria, string tokenId,
                    out string apiRequest, out string apiResponse);
    }
}