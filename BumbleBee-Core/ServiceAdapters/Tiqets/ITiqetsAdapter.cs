using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Tiqets;
using ServiceAdapters.Tiqets.Tiqets.Entities;
using ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Net;
using BookingRequest = Isango.Entities.Tiqets.BookingRequest;
using Product = ServiceAdapters.Tiqets.Tiqets.Entities.Product;
using TiqetsCriteria = Isango.Entities.Tiqets.TiqetsCriteria;

namespace ServiceAdapters.Tiqets
{
    public interface ITiqetsAdapter
    {
        Product GetProductDetailsByProductId(int productId, string language, string token);

        List<ProductOption> GetPriceAndAvailabilityByProductId(TiqetsCriteria criteria, string token);

        List<ProductOption> GetAvailabilities(TiqetsCriteria criteria, string token);

        List<ProductVariant> GetVariantsForDumpingApplication(TiqetsCriteria criteria, string token);

        BulkAvailabilityResponse GetBulkAvailabilityByProductId(int productId, DateTime startDate, DateTime endDate, string token);

        CreateOrderResponse CreateOrder(BookingRequest bookingRequest, string token, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode);

        ConfirmOrderResponse ConfirmOrder(BookingRequest bookingRequest, string token, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode);

        OrderInformationResponse GetOrderInformation(TiqetsSelectedProduct tiqetsProduct, string bookingReferenceNo, string token, string languageCode, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode, string affiliateId = "");

        bool CancelOrder(TiqetsSelectedProduct tiqetsProduct, string bookingReferenceNo, string token, string languageCode, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode, string affiliateId = "");
        Booking GetTicket(BookingRequest bookingRequest, string token, out string apiRequest, out string apiResponse, out HttpStatusCode httpStatusCode);

        ProductFilter GetProductFilter(string token, int pageNumber);
    }
}