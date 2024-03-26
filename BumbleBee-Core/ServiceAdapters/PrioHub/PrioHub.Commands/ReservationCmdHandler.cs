using Logger.Contract;
using ServiceAdapters.PrioHub.Constants;
using ServiceAdapters.PrioHub.PrioHub.Commands.Contract;
using ServiceAdapters.PrioHub.PrioHub.Entities;
using ServiceAdapters.PrioHub.PrioHub.Entities.ReservationRequest;
using Util;

namespace ServiceAdapters.PrioHub.PrioHub.Commands
{
    public class ReservationCmdHandler : CommandHandlerBase, IReservationCommandHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly ReservationRequest _reservationRq;

        public ReservationCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _reservationRq = new ReservationRequest();
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var returnData = inputContext as InputContext;
            var apiActivityId = returnData?.ActivityId;
            var distributorId = returnData.PrioHubDistributerId;
            var bookingReference = returnData?.BookingReference;
            var prioHubAvailabilityId = returnData?.PrioHubAvailabilityId;
            var prioHubProductPaxMapping = returnData?.PrioHubProductPaxMapping;
            var ticketType = returnData?.TicketType;
            var ticketTypeCount = returnData?.Count;
            var pickupPointId = returnData?.PickupPointId;
            var pickupPoints = returnData?.PickupPoints;
            var productCombiDetails = returnData?.ProductCombiDetails;
            var prioHubComboSubProduct = returnData?.PrioHubComboSubProduct;
            var prioHubClusterProduct = returnData?.PrioHubClusterProduct;

            var reservationDataRq = _reservationRq;
            reservationDataRq.Data = new Entities.ReservationRequest.Data
            {
                Reservation = new Entities.ReservationRequest.Reservation()
            };
            var reservationRq = reservationDataRq.Data.Reservation;
            reservationRq.ReservationDistributorId = distributorId;
            reservationRq.ReservationExternalReference = bookingReference;
            reservationRq.ReservationDetails = new List<ReservationDetails>();

            var reservationDetails = new ReservationDetails
            {
                BookingExternalReference = bookingReference,
                BookingLanguage = "en",
                ProductId = apiActivityId,
                ProductAvailabilityId = prioHubAvailabilityId,
                ProductTypeDetails = new List<Entities.ReservationRequest.ProductTypeDetails>(),
                ProductCombiDetails = new List<ProductCombiDetail>()
            };

            //Cluster Product
            if (prioHubClusterProduct != null)
            {
                reservationDetails.ProductId =Convert.ToString(prioHubClusterProduct?.ProductId);
                reservationDetails.ProductRelationId = Convert.ToString(prioHubClusterProduct?.ProductParentId);
            }
            //Combi Products
            if (productCombiDetails != null && productCombiDetails.Count > 0)
            {
                foreach (var productCombiItem in productCombiDetails)
                {
                    var item = new ProductCombiDetail
                    {
                        ProductId = productCombiItem?.ProductId
                    };
                    if (prioHubComboSubProduct != null && prioHubComboSubProduct.Count > 0)
                    {
                        item.ProductAvailabilityId = prioHubComboSubProduct?.Where(x => x.AvailabilityProductId == productCombiItem?.ProductId)?.FirstOrDefault()?.AvailabilityId;
                    }
                    reservationDetails.ProductCombiDetails.Add(item);
                }
            }
            else
            {
                reservationDetails.ProductCombiDetails = null;
            }
            //Adult,Child, Infant etc

            for (int i = 0; i <= ticketType?.Count - 1; i++)
            {
                var productTypeDetails = new Entities.ReservationRequest.ProductTypeDetails
                {
                    ProductTypeId = ticketType[i],
                    ProductTypeCount = ticketTypeCount[i]
                };
                var passengerText = productTypeDetails?.ProductTypeId;
                //Assign id from mapping
                if (prioHubProductPaxMapping != null && prioHubProductPaxMapping.Count > 0)
                {
                    var productTypeClass = prioHubProductPaxMapping?.FirstOrDefault()?.ProductTypeClass;
                    //Standard(Adult,Child, Infant,senior, youth), GROUP
                    if (productTypeClass?.ToUpper() ==Convert.ToString(ProductTypeClass.STANDARD))
                    {
                        var passengerIdGet = prioHubProductPaxMapping?.Where(x => x.ProductType.ToUpper() == passengerText.ToUpper())?.FirstOrDefault()?.ProductTypeId;
                        productTypeDetails.ProductTypeId = passengerIdGet;
                    }
                    //Product types in the individual class are less common and therefore have fewer supported systems.
                    //These types will never be age-restricted.
                    //INDIVIDUAL:PERSON,STUDENT,RESIDENT,MILITARY,IMPAIRED.
                    //It have only one paxtype (ADULT)
                    else
                    {
                        var passengerIdGet = prioHubProductPaxMapping?.FirstOrDefault()?.ProductTypeId;
                        productTypeDetails.ProductTypeId = passengerIdGet;
                    }
                }

                reservationDetails?.ProductTypeDetails?.Add(productTypeDetails);
            }

            reservationDetails.ProductOptions = null;
            if (pickupPoints == "MANDATORY")
            {
                reservationDetails.ProductPickupPointId = pickupPointId;
                reservationDetails.ProductPickupPoint = new ProductPickupPoint
                {
                    PickupPointId = pickupPointId
                };
            }
            
            reservationRq.ReservationDetails.Add(reservationDetails);

            var jsonRequest = SerializeDeSerializeHelper.Serialize(_reservationRq);
            return jsonRequest;
        }

        protected override object GetResultsAsync(object inputContext)
        {
            var prioHubCriteriaReservationRQ = SerializeDeSerializeHelper.DeSerialize<ReservationRequest>(inputContext.ToString());
            var scope = _PrioHubApiScopeReservation;
            var actualDistributerId = prioHubCriteriaReservationRQ?.Data?.Reservation?.ReservationDistributorId;
            //1. Using basic Auth Get AccessToken 
            var accessToken = AddRequestHeadersAndAddressToApi(scope,Convert.ToInt32(actualDistributerId));
            
            var actualBaseAddress = _PrioHubServiceURL;
            //if (actualDistributerId == _PrioHubApiDistributorIdPrioOnly)
            //{
            //    actualBaseAddress = _PrioHubServiceURLOnlyPrioProducts;
            //}
            var baseAddress = actualBaseAddress + Constant.Reservations;
            //2. Using Bearer"
            var client = new AsyncClient
            {
                ServiceURL = baseAddress
            };
            var headers = new Dictionary<string, string>
            {
                {Constant.Authorization, $"{Constant.Bearer}{accessToken}"},
                {Constant.Accept, Constant.App_Json},
                {Constant.Content_type, Constant.App_Json}
            };
            var returnData = client.PostJsonWithHeadersAsync((ReservationRequest)prioHubCriteriaReservationRQ, headers)?.GetAwaiter().GetResult();
            return returnData;
        }
        

       

        ///// <summary>
        ///// Get Results
        ///// </summary>
        ///// <param name="jsonResult"></param>
        ///// <returns></returns>
        //protected override object GetResults(object jsonResult)
        //{
        //    return SerializeDeSerializeHelper.DeSerialize<ReservationResponse>(jsonResult.ToString());
        //}
        

        protected override object GetResponseObject(string responseText)
        {
            return responseText;
        }
    }
}