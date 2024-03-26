using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.GoogleMaps;
using System.Collections.Generic;

namespace Isango.Service.Contract
{
    public interface IGoogleMapsDataDumpingService
    {
        int DumpPriceAndAvailabilities(List<TempHBServiceDetail> serviceDetails, List<ProductOption> productOptions, APIType apiType);

        ExtraDetail DumpExtraDetail(List<Activity> activities, APIType apiType);

        void DumpCancellationPolicies(List<GoogleCancellationPolicy> cancellationPoliciesForGoogle);

        Entities.GoogleMaps.ExtraDetail GetBokunExtraDetail(List<Activity> activities, string token);

        Entities.GoogleMaps.ExtraDetail GetGoldenToursExtraDetail(List<Activity> activities, string token);

        Entities.GoogleMaps.ExtraDetail GetGrayLineIceLandExtraDetail(List<Activity> activities, string token);
    }
}