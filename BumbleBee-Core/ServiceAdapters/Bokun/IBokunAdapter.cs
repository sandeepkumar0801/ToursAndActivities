using Isango.Entities.Bokun;
using Isango.Entities.Booking;
using ServiceAdapters.Bokun.Bokun.Entities.GetActivity;
using ServiceAdapters.Bokun.Bokun.Entities.GetBooking;
using ServiceAdapters.Bokun.Bokun.Entities.GetPickupPlaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Question = Isango.Entities.Bokun.Question;

namespace ServiceAdapters.Bokun
{
    public interface IBokunAdapter
    {
        GetBookingRs GetBooking(string bookingId, string token);

        Task<GetBookingRs> GetBookingAsync(string bookingId, string token);

        List<Isango.Entities.Activities.Activity> CheckAvailabilities(BokunCriteria criteria, Isango.Entities.Activities.Activity activity, string token);

        Task<List<Isango.Entities.Activities.Activity>> CheckAvailabilitiesAsync(BokunCriteria criteria, Isango.Entities.Activities.Activity activity, string token);

        List<Question> CheckoutOptions(BokunSelectedProduct request, string token);

        Task<List<Question>> CheckoutOptionsAsync(BokunSelectedProduct request, string token);

        Booking SubmitCheckout(BokunSelectedProduct request, string token);

        Task<Booking> SubmitCheckoutAsync(BokunSelectedProduct request, string token);

        bool CancelBooking(string confirmationCode, string token);

        Task<bool> CancelBookingAsync(string confirmationCode, string token);

        GetActivityRs GetActivity(string factsheetId, string token);

        Task<GetActivityRs> GetActivityAsync(BokunCriteria request, string token);

        string EditBooking(BokunSelectedProduct request, string token);

        Task<string> EditBookingAsync(BokunSelectedProduct request, string token);

        bool CancelBooking(string confirmationCode, string token, out string apiRequest, out string apiResponse);

        Booking SubmitCheckout(BokunSelectedProduct request, string token, out string apiRequest, out string apiResponse);

        GetPickupPlacesRS GetPickupPlaces(int activityId, string token);

        Task<GetPickupPlacesRS> GetPickupPlacesAsync(int activityId, string token);

        GetActivityRs GetActivity(BokunCriteria request, string token, out string apiRequest, out string apiResponse);

        List<Isango.Entities.Activities.Activity> CheckAvailabilitiesForDumpingApp(BokunCriteria criteria, Isango.Entities.Activities.Activity activity, string token);
    }
}