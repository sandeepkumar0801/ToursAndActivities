using Isango.Entities;
using Isango.Entities.Redeam;
using ServiceAdapters.Redeam.Redeam.Commands.Contracts;
using ServiceAdapters.Redeam.Redeam.Converters.Contracts;
using ServiceAdapters.Redeam.Redeam.Entities;
using ServiceAdapters.Redeam.Redeam.Entities.CreateHold;
using ServiceAdapters.Redeam.Redeam.Entities.GetAvailability;
using ServiceAdapters.Redeam.Redeam.Entities.GetRate;
using ServiceAdapters.Redeam.Redeam.Entities.GetRates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Constant = ServiceAdapters.Redeam.Constants.Constant;

namespace ServiceAdapters.Redeam
{
    public class RedeamAdapter : IRedeamAdapter
    {
        private readonly IGetAvailabilitiesCommandHandler _getAvailabilitiesCommandHandler;
        private readonly IGetRateCommandHandler _getRateCommandHandler;
        private readonly IGetRatesCommandHandler _getRatesCommandHandler;
        private readonly IGetAvailabilityCommandHandler _getAvailabilityCommandHandler;
        private readonly IGetSuppliersCommandHandler _getSuppliersCommandHandler;
        private readonly IGetProductsCommandHandler _getProductsCommandHandler;
        private readonly ICreateHoldCommandHandler _createHoldCommandHandler;
        private readonly IDeleteHoldCommandHandler _deleteHoldCommandHandler;
        private readonly ICreateBookingCommandHandler _createBookingCommandHandler;
        private readonly ICancelBookingCommandHandler _cancelBookingCommandHandler;

        private readonly IGetAvailabilitiesConverter _getAvailabilitiesConverter;
        private readonly IGetSuppliersConverter _getSuppliersConverter;
        private readonly IGetProductsConverter _getProductsConverter;
        private readonly IGetRatesConverter _getRatesConverter;
        private readonly ICreateHoldConverter _createHoldConverter;
        private readonly ICreateBookingConverter _createBookingConverter;

        public RedeamAdapter(
            IGetAvailabilitiesCommandHandler getAvailabilitiesCommandHandler,
            IGetRateCommandHandler getRateCommandHandler,
            IGetRatesCommandHandler getRatesCommandHandler,
            IGetAvailabilityCommandHandler getAvailabilityCommandHandler,
            IGetSuppliersCommandHandler getSuppliersCommandHandler,
            IGetProductsCommandHandler getProductsCommandHandler,
            ICreateHoldCommandHandler createHoldCommandHandler,
            IDeleteHoldCommandHandler deleteHoldCommandHandler,
            ICreateBookingCommandHandler createBookingCommandHandler,
            ICancelBookingCommandHandler cancelBookingCommandHandler,
            IGetAvailabilitiesConverter getAvailabilitiesConverter,
            IGetSuppliersConverter getSuppliersConverter,
            IGetProductsConverter getProductsConverter,
            IGetRatesConverter getRatesConverter,
            ICreateHoldConverter createHoldConverter,
            ICreateBookingConverter createBookingConverter)
        {
            _getAvailabilitiesCommandHandler = getAvailabilitiesCommandHandler;
            _getRateCommandHandler = getRateCommandHandler;
            _getRatesCommandHandler = getRatesCommandHandler;
            _getAvailabilityCommandHandler = getAvailabilityCommandHandler;
            _getSuppliersCommandHandler = getSuppliersCommandHandler;
            _getProductsCommandHandler = getProductsCommandHandler;
            _createHoldCommandHandler = createHoldCommandHandler;
            _deleteHoldCommandHandler = deleteHoldCommandHandler;
            _createBookingCommandHandler = createBookingCommandHandler;
            _cancelBookingCommandHandler = cancelBookingCommandHandler;

            _getAvailabilitiesConverter = getAvailabilitiesConverter;
            _getSuppliersConverter = getSuppliersConverter;
            _getProductsConverter = getProductsConverter;
            _getRatesConverter = getRatesConverter;
            _createHoldConverter = createHoldConverter;
            _createBookingConverter = createBookingConverter;
        }

        #region Public Methods

        /// <summary>
        /// Fetch availabilities asynchronously
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<ProductOption>> GetAvailabilities(RedeamCriteria criteria, string token)
        {
            var rateIds = criteria.RateIds;
            var productOptions = new List<ProductOption>();

            foreach (var ProductAndRateId in rateIds)
            {
                try
                {
                    //Continue the loop if the productId is null or empty
                    if (string.IsNullOrWhiteSpace(ProductAndRateId)) continue;

                    var productAndRateId = ProductAndRateId.Split('#');
                    criteria.ProductId = productAndRateId[0];
                    criteria.RateId = productAndRateId[1];
                    var rateType = criteria.RateIdAndType?.FirstOrDefault(x => x.Key == ProductAndRateId).Value;
                    if (string.IsNullOrEmpty(rateType)) continue;
                    object availabilitiesResponse = null;

                    //call this method if type is RESERVED
                    if (rateType == Constant.ReservedType)
                    {
                        availabilitiesResponse =
                       _getAvailabilitiesCommandHandler.Execute(criteria, MethodType.GetAvailabilities,
                           token);
                    }

                    // GetRate call, continue if response is null
                    var ratesResponse = await GetSingleRate(criteria, token);
                    if (ratesResponse == null) continue;

                    // Pass both the responses and the criteria in the converter to prepare product options
                    var result =
                       _getAvailabilitiesConverter.Convert(availabilitiesResponse, criteria, ratesResponse) as
                           List<ProductOption>;
                    if (result.Count == 0) continue;
                    productOptions.AddRange(result);
                }
                catch (Exception ex)
                {
                    //ignored //#TODO Add logging here
                }
            }
            return await Task.FromResult(productOptions);
        }

        /// <summary>
        /// Fetch rates asynchronously (Change the return type if this method is called on service layer as GetRateResponse is a ServiceAdapter model)
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<GetRatesResponse> GetRates(RedeamCriteria criteria, string token)
        {
            var result = _getRatesCommandHandler.Execute(criteria, MethodType.GetRates, token);
            if (result == null) return null;

            var response = SerializeDeSerializeHelper.DeSerialize<GetRatesResponse>(result.ToString());
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Calls the GetRates API and prepares the RatesWrapper
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<RatesWrapper> GetRatesWrapper(RedeamCriteria criteria, string token)
        {
            var ratesResponse = await GetRates(criteria, token);
            if (ratesResponse == null) return null;

            var ratesWrapper = _getRatesConverter.Convert(ratesResponse) as RatesWrapper;
            return await Task.FromResult(ratesWrapper);
        }

        /// <summary>
        /// Fetch all suppliers asynchronously
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<SupplierData>> GetSuppliers(string token)
        {
            var result = _getSuppliersCommandHandler.Execute("", MethodType.GetSuppliers, token);
            if (result == null) return null;

            var response = _getSuppliersConverter.Convert(result) as List<SupplierData>;
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Fetch all products asynchronously
        /// </summary>
        /// <param name="token"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<List<ProductData>> GetProducts(RedeamCriteria criteria, string token)
        {
            var result = _getProductsCommandHandler.Execute(criteria, MethodType.GetProducts, token);
            if (result == null) return null;

            var response = _getProductsConverter.Convert(result, criteria.SupplierId) as List<ProductData>;
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Acquire hold asynchronously
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<SelectedProduct> CreateHold(SelectedProduct selectedProducts, string token)
        {
            var result = _createHoldCommandHandler.Execute(selectedProducts, MethodType.CreateHold, token);
            if (result == null) return null;

            var response = _createHoldConverter.Convert(result, selectedProducts) as SelectedProduct;
            return await Task.FromResult(response);
        }

        public async Task<CreateHoldResponse> CreateHoldAPIOnly(SelectedProduct selectedProducts, string token)
        {
            var result = _createHoldCommandHandler.Execute(selectedProducts, MethodType.CreateHold, token);
            if (result == null) return null;

            var response = SerializeDeSerializeHelper.DeSerialize<CreateHoldResponse>(result.ToString());
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Delete hold asynchronously
        /// </summary>
        /// <param name="holdIds"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> DeleteHold(List<string> holdIds, string token)
        {
            var statusByHoldId = new Dictionary<string, string>();
            foreach (var holdId in holdIds)
            {
                var result =
                    _deleteHoldCommandHandler.Execute(holdId, MethodType.DeleteHold, token);

                if (statusByHoldId.Keys.Contains(holdId)) continue;
                statusByHoldId.Add(holdId, result == null ? "" : Constant.ReleasedStatus);
            }

            return await Task.FromResult(statusByHoldId);
        }

        /// <summary>
        /// Create Booking asynchronously
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<SelectedProduct> CreateBooking(SelectedProduct selectedProducts, string token)
        {
            var result = _createBookingCommandHandler.Execute(selectedProducts, MethodType.CreateBooking, token);
            if (result == null) return null;

            var response = _createBookingConverter.Convert(result, selectedProducts) as SelectedProduct;
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Create Booking
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="token"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public SelectedProduct CreateBooking(SelectedProduct selectedProducts, string token, out string apiRequest, out string apiResponse)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;

            var result = _createBookingCommandHandler.Execute(selectedProducts, MethodType.CreateBooking, token, out apiRequest, out apiResponse);

            //Mocking api booking
            /*
            var isMock = false;
            apiRequest = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\13 Redeam\03  BookingRequest.json");
            apiResponse = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\13 Redeam\04 BookingResponse.json");
            var bookingConfirmRS = (object)apiResponse;
            var result = bookingConfirmRS;
            isMock = true;
            //*/

            if (result == null) return null;

            var response = _createBookingConverter.Convert(result, selectedProducts);
            return response as SelectedProduct;
        }

        /// <summary>
        /// Cancel Booking asynchronously
        /// </summary>
        /// <param name="bookingReferenceNumbers"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> CancelBooking(List<string> bookingReferenceNumbers, string token)
        {
            var statusByBookingReferenceNumber = new Dictionary<string, string>();
            foreach (var bookingReferenceNumber in bookingReferenceNumbers)
            {
                var result = _cancelBookingCommandHandler.Execute(bookingReferenceNumber, MethodType.CancelBooking, token);

                if (statusByBookingReferenceNumber.Keys.Contains(bookingReferenceNumber)) continue;
                statusByBookingReferenceNumber.Add(bookingReferenceNumber, result == null ? "" : Constant.CancelledStatus);
            }

            return await Task.FromResult(statusByBookingReferenceNumber);
        }

        /// <summary>
        /// Cancel Booking
        /// </summary>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="token"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public bool CancelBooking(string bookingReferenceNumber, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var result = _cancelBookingCommandHandler.Execute(bookingReferenceNumber, MethodType.CancelBooking, token, out request, out response);
            return result == null;
        }

        #endregion Public Methods

        #region Not used in the current flow

        /// <summary>
        /// Fetch single rate asynchronously
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<GetRateResponse> GetSingleRate(RedeamCriteria criteria, string token)
        {
            var result = _getRateCommandHandler.Execute(criteria, MethodType.GetRate, token);
            if (result == null) return null;
            var response = SerializeDeSerializeHelper.DeSerialize<GetRateResponse>(result.ToString());
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Fetch single availability (Change the return type if this method is called on service layer as GetRateResponse is a ServiceAdapter model)
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public AvailabilityResponse GetSingleAvailability(RedeamCriteria criteria, string token)
        {
            var result = _getAvailabilityCommandHandler.Execute(criteria, MethodType.GetAvailability, token);
            if (result == null) return null;
            var response = SerializeDeSerializeHelper.DeSerialize<AvailabilityResponse>(result.ToString());
            return response;
        }

        #endregion Not used in the current flow
    }
}