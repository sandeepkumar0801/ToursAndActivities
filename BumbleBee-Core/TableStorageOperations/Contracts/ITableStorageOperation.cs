using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.Booking.RequestModels;
using Isango.Entities.GoogleMaps.BookingServer;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using TableStorageOperations.Models;
using TableStorageOperations.Models.AdditionalPropertiesModels;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using TableStorageOperations.Models.Booking;

namespace TableStorageOperations.Contracts
{
    public interface ITableStorageOperation
    {
        void InsertData(Activity activity, string tokenId);

        T RetrieveData<T>(string referenceId, string partitionKey) where T : ITableEntity, new();

        TableEntity Retrieve(string referenceId, string partitionKey, int apiTypeId);

        void InsertBookingRequest(CreateBookingRequest request, string bookingGuid, string bookingReferenceNumber,BookingResponse bookingResponse= null, string error = null,string statusCode = "500",string token="");

        Tuple<string, CreateBookingRequest> RetrieveBookingRequest(string bookingGuid);

        List<PriceOffer> RetrievePriceOfferData(string partitionKey, string rowKey);

        T RetrieveData<T>(string partitionKey, string rowKey, string tableName) where T : ITableEntity, new();

        void InsertOrderResponse(Order order, string tokenId);

        Order GetOrders(string columnName, string queryParameter);

        void InsertBookingCallBackRequest(string bookingGuid, string bookingReferenceNumber, string status, BookingResponse bookingResponse, string token,string error = null, string statusCode = "500");

        Tuple<string, BookingResponse> RetrieveBookingCallBackRequest(string bookingGuid);

        void InsertAsyncBookingDetails(AsyncBooking asyncBooking);

        List<AsyncBooking> RetrieveAsyncBooking(int apiType, string azureTableColumnName, string azureTableColumnValue);

        void UpdateTiqetsGetTicketData(TiqetsTableStorageData tiqetsTableStorageData, AsyncBooking asyncBooking);

        void InsertTiqetsGetTicketDetailsLog(string asyncBookingId, string orderReferenceId, string status, string processedDatetime, string request, string response, string requestType);

        void InsertRezdyPickUpLocations(List<Isango.Entities.Rezdy.RezdyPickUpLocation> rezdyPickUpLocations,
            int pickUpId);

        Isango.Entities.Rezdy.RezdyPickUpLocation RetrievePickUpLocation(int pickUpId, string pickUpLocationId);
        void UpdateGlobalTixData(GlobalTixAvailabilities globalTixAvailabilities);
        void InsertReservationDetails(SupplierBookingReservationResponse reservationDetails);

        SupplierBookingReservationResponse RetrieveReservationData(string rowKey);

        void InsertReservationRequest(CreateBookingRequest request, string bookingGuid,
            string bookingReferenceNumber, ReservationResponse reservationResponse = null, string error = null,
            string statusCode = "500", string token = "");

        List<AsyncBooking> RetrieveFailedWebhookAsyncBooking(int apiType, string azureTableColumnName, string azureTableColumnValue);

        List<AsyncBooking> RetrieveAsyncBookingRowKey(int apiType, string azureTableColumnName, string azureTableColumnValue);

        void UpdatePrioHubWebhookData(string webhookResponse,
           int webhookRetryCount, string webhookURL, AsyncBooking asyncBooking, string isWebhookSuccess);

        List<AvailabilityModelResponse> RetrieveAdapterLoggingData(string methodName, string apiName, string PartitionKey);
    }
}