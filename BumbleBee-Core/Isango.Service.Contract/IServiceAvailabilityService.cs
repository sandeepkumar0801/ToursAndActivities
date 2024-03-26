using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.GoogleMaps;
using System;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface IServiceAvailabilityService
    {
        void SaveServiceAvailabilitiesInDatabase(List<TempHBServiceDetail> serviceDetails);

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetGrayLineIceLandAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetMoulinRougeAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetPrioAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetFareHarborAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetBokunAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetAotAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetTiqetsAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetVentrataAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetGoldenToursAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetIsangoAvailabilities();

        List<GoogleCancellationPolicy> GetCancellationPolicies();

        void SaveQuestionsInDatabase(List<ExtraDetailQuestions> Questions, int ApiType);

        Tuple<List<Activity>, List<TempHBServiceDetail>> SaveApiTudeAvailabilities(List<IsangoHBProductMapping> products);

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetRezdyServiceDetails();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetRedeamServiceDetails();

        Tuple<List<Activity>, List<TempHBServiceDetail>> SaveGlobalTixAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetTourCMSAvailabilities();
        void SaveRedeamAvailabilities();

        void DeleteExistingAvailabilityDetails();

        void SyncPriceAvailabilities();

        void SaveRezdyAvailabilities();

        List<string> GetAgeDumpingAPIs();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetNewCitySightSeeingServiceDetails();
        List<Entities.Ventrata.SupplierDetails> GetVentrataData();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetPrioHubAvailabilities();

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetRaynaServiceDetails();

        void SaveServiceAvailabilitiesInDatabaseForTiqets(List<TempHBServiceDetail> serviceDetails);

        Tuple<List<Activity>, List<TempHBServiceDetail>> GetRedeamV12ServiceDetails();

        Tuple<List<Activity>, List<TempHBServiceDetail>> SaveGlobalTixV3Availabilities();
    }
}