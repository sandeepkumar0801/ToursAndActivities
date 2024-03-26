using Isango.Entities;
using Isango.Entities.Bokun;
using Isango.Entities.HotelBeds;
using Isango.Entities.PrioHub;
using Isango.Entities.Rezdy;
using Isango.Entities.TourCMS;
using Isango.Entities.Ventrata;
using System;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface ICartService
    {
        ExtraDetailsForBokun GetExtraDetailsForBokun(BokunSelectedProduct selectedProduct, string token);

        List<PickupLocation> GetExtraDetailsForGrayLineIceLand(int activityId, string token);

        HotelBedsSelectedProduct GetExtraDetailsForHotelBeds(HotelBedsSelectedProduct selectedProduct, string token);
        ExtraDetailsForRezdy GetExtraDetailsForRezdy(string ProductCode, int passengerCount, string token);
        Tuple<List<ContractQuestion>, Dictionary<int, string>> GetExtraDetailsForGlobalTix(List<string> ticketTypeIds, string token);

        ExtraDetailsForTourCMS GetExtraDetailsForTourCMS(TourCMSSelectedProduct selectedProduct,
        List<PickupPointsForTourCMS> pickupPointsForTourCMSList,
        List<QuestionsForTourCMS> questionsForTourCMSList, string token);
        ExtraDetailsForPrioHub GetExtraDetailsForPrioHub(
    List<PickUpPointForPrioHub> pickUpPointForPrioHubList,
    string token);

        Dictionary<int, string> GetExtraDetailsForFareHarbor(string availabilityPK, string supplierName, string userKey, string token);

        List<VentrataExtraQuestion> GetCustomQuestionsForVentrata(string supplierBearerToken, string baseURL,
           string ventrataProductId, string token, string supplierOptionCode);
    }
}