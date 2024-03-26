using System.Threading.Tasks;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;


namespace Isango.Service.Contract
{
    public interface IAgeGroupService
    {
        void SaveAOTAgeGroups(string token);

        Task SaveGrayLineIceLandAgeGroups(string token);

        Task SaveGrayLineIceLandPickupLocations(string token);

        void SyncGrayLineIceLandData();

        void SaveFareHarborCompanies(string token);

        void SaveFareHarborCustomerProtoTypes(string token);

        void SyncFareHarborData();

        void SavePrioTicketDetails(string token);

        void SaveTiqetsVariants(string token);

        void SaveGoldenToursAgeGroups(string token);

        void SaveBokunAgeGroups(string token);

        void SaveAPITudeContentData(string token);

        void SaveRedeamData(string token);
        void SaveRezdyDataInDB(string token);
        void SaveGlobalTixCountryCityList(string token);

        void SaveGlobalTixActivities(string token);

        void SaveGlobalTixPackages(string token);
        void SaveVentrataProductDetails(string token);

        void SaveTourCMSChannelData(string token);
        void SaveTourCMSTourData(string token);


        void SaveNewCitySightSeeingProductList(string token);

        void SaveGoCityProductList(string token);
        void SavePrioHubProductData(string token);

        void SaveRaynaProductList(string token);

        void SaveExternalProducts(string token);

        CssBookingResponseResult CreateBooking(string token, CreateBookingRequest bookingRequest);
        void csswebjob();
        object CancelBooking(string token, CancellationRequest cancellationRequest);

        void csswebjobCancellation();
        void SaveCssRedemptionBooking(string tokenId);
        void csswebjobRedemption();

        void SaveRedeamV12Data(string token);

        void SaveGlobalTixCountryCityListV3(string token);
        void SaveGlobalTixCategoriesV3(string token);
        void SaveGlobalTixProductInfoListV3(string token);
        Task SaveGlobalTixActivitiesV3Async(string token);

        void SaveGlobalTixV3PackageOptions(string token);
       
    }
}