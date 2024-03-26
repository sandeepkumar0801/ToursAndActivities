using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.DataDumping;
using System.Collections.Generic;
using Isango.Entities.GoogleMaps;

namespace Isango.Service.Contract
{
    public interface IStorageOperationService
    {
        void InsertExtraDetail(Entities.GoogleMaps.ExtraDetail extraDetail);

        void InsertServiceDetails(List<StorageServiceDetail> storageServiceDetails);

        void InsertCancellationPolicy(List<GoogleCancellationPolicy> cancellationPolicies);

       List<StorageServiceDetail> GetServiceDetails(string partitionKey);

        List<Entities.ConsoleApplication.DataDumping.ExtraDetail> GetExtraDetails(string partitionKey);

       List<CancellationPolicy> GetCancellationPolicies(string partitionKey);

        void AddMessageToQueue(string queueName, string message);

        List<OrderResponse> GetCancelledOrders();

       void UpdateNotifiedOrders(List<OrderResponse> orderResponses);
    }
}