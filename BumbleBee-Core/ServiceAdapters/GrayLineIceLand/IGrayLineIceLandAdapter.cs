using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.GrayLineIceLand;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgrGroup = Isango.Entities.ConsoleApplication.AgeGroup.GrayLineIceLand;

namespace ServiceAdapters.GrayLineIceLand
{
    public interface IGrayLineIceLandAdapter
    {
        Task<List<Activity>> GetAvailabilityAndPriceAsync(GrayLineIcelandCriteria criteria, string token);

        List<Activity> GetAvailabilityAndPrice(GrayLineIcelandCriteria criteria, string token);

        Task<Booking> CreateBookingAsync(List<GrayLineIceLandSelectedProduct> selectedProducts, string token);

        Booking CreateBooking(List<GrayLineIceLandSelectedProduct> selectedProducts, string bookingReference, string token, out string requestJson, out string responseJson);

        Boolean DeleteBooking(string supplierReferenceNumber, string token, out string requestXml, out string responseXml);

        Task<Boolean> DeleteBookingAsync(Booking booking, string token);

        Task<Dictionary<int, List<AgrGroup.AgeGroup>>> GetAgeGroupsByToursAsync(List<IsangoHBProductMapping> products, string token);

        Task<Dictionary<int, List<AgrGroup.Pickuplocation>>> GetAllPickupLocationsAsync(List<IsangoHBProductMapping> grayLineProducts, string token);
    }
}