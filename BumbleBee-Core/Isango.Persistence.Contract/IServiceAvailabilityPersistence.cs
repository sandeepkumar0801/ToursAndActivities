using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.GoogleMaps;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IServiceAvailabilityPersistence
    {
        void DeleteExistingHBServiceDetails();

        void SyncAPIPriceAvailabilities();

        void SaveErrorLogs(ErrorLogger error);

        void SaveServiceAvailabilities(List<TempHBServiceDetail> serviceDetails);

        void SaveQuestionsInDB(List<ExtraDetailQuestions> Questions, int ApiType);

        List<ServiceAvailabilityFeed> GetIsangoServiceAvailabilities();

        void SaveServiceAvailabilitiesGTix(List<TempHBServiceDetail> serviceDetails);

        void SaveServiceAvailabilitiesForTiqets(List<TempHBServiceDetail> serviceDetails);
    }
}