using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Service.Canocalization
{
    public interface ICanocalizationService
    {
        #region [Data Dumping Queue or Static Data Dumping] 
        object GetAgeGroupData(string token, object data, string methodType, APIType apitype);
        #endregion


        #region [Data Dumping or Calendar Data Dumping ] 
        List<Activity> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria criteria);
        List<Entities.ConsoleApplication.ServiceAvailability.TempHBServiceDetail> GetServiceDetails(List<Activity> activities,
            List<IsangoHBProductMapping> mappedProducts, PriceDataType priceDataType, APIType apiType);
        #endregion

        #region [CheckAvailability]
        Activity GetAvailability(Activity activity, CanocalizationCriteria criteria, string token);
        #endregion

        #region[Reservation]
        List<BookedProduct> CreateReservation(CanocalizationActivityBookingCriteria criteria);
        #endregion
        #region [CreateBooking]
        List<BookedProduct> CreateBooking(CanocalizationActivityBookingCriteria criteria);
        #endregion

        #region[Cancellation]
        Dictionary<string, bool> CancelBooking(List<SelectedProduct> selectedProducts, string token,APIType apitype);
        #endregion

        CanocalizationCriteria CreateCriteria(Activity activity, Criteria criteria, ClientInfo clientInfo);
    }
}
