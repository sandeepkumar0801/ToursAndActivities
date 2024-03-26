using Logger.Contract;
using ServiceAdapters.PrioHub.Constants;
using ServiceAdapters.PrioHub.PrioHub.Commands.Contract;
using ServiceAdapters.PrioHub.PrioHub.Entities;
using ServiceAdapters.PrioHub.PrioHub.Entities.CreateBookingRequest;
using ServiceAdapters.PrioHub.PrioHub.Entities.CustomHelper;
using System.Text;
using System.Text.Json;
using Util;

namespace ServiceAdapters.PrioHub.PrioHub.Commands
{
    public class CreateBookingCmdHandler : CommandHandlerBase, ICreateBookingCommandHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly CreateBookingRequest _createBookingRq;

        public CreateBookingCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _createBookingRq = new CreateBookingRequest();
        }

        // Command Handler are use to Create Input Request , Call Api and GetResult and convert it into the DTO in Entities
        /// <summary>
        /// Create Input Request
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            //throw new NullReferenceException("for testing"); //for reservation cancel testing
            var returnData = inputContext as InputContext;

            var customers = returnData?.Customers;

            var bookingDataRq = _createBookingRq;
            bookingDataRq.Data = new Entities.CreateBookingRequest.Data
            {
                Order = new Entities.CreateBookingRequest.Order()
            };
            var bookingRq = bookingDataRq.Data.Order;
            bookingRq.OrderBookings = new List<Entities.CreateBookingRequest.OrderBookings>();
            bookingRq.OrderContacts = new List<Entities.CreateBookingRequest.OrderContacts>();
            bookingRq.OrderCustomFields = new List<Entities.CreateBookingRequest.OrderCustomFields>();
            bookingRq.OrderOptions = new Entities.CreateBookingRequest.OrderOptions();

            bookingRq.OrderExternalReference = returnData?.BookingReference + "_" + Convert.ToString(returnData?.ActivityId);
            bookingRq.OrderLanguage = returnData?.Language;
            bookingRq.OrderSettlementType = "";
            bookingRq.OrderDistributorId = returnData.PrioHubDistributerId;
            
            //start- orderBookings
            var orderBookings = new Entities.CreateBookingRequest.OrderBookings
            {
                BookingExternalReference = returnData?.BookingReference + "_" + Convert.ToString(returnData?.ActivityId),
                BookingOptionType = returnData?.BookingOptionType,
                ReservationReference = returnData?.ReservationReference
            };
            bookingRq.OrderBookings.Add(orderBookings);
            //end- orderBookings

            //start- orderContacts
            var orderContacts = new Entities.CreateBookingRequest.OrderContacts
            {
                ContactEmail = returnData?.BookingEmail,
                ContactNameFirst = returnData?.BookingFirstName,
                ContactNameLast = returnData?.BookingLastName
            };
            bookingRq.OrderContacts.Add(orderContacts);
            //end- orderContacts

            //start- orderCustomFields
            

            if (customers != null && customers.Count > 1)
            {
                var productTypeIdLead = string.Empty;
                var customFieldValue = new StringBuilder();
                var prioHubProductPaxMapping = returnData?.PrioHubProductPaxMapping;
                var dateString = returnData?.TourDate.ToString("yyyy-MM-dd");
                var groupedCustomerList = customers?.GroupBy(u => u.PassengerType)?.Select(grp => grp.ToList())?.ToList();
   
                foreach (var itemCustomerGroup in groupedCustomerList)
                {
                    var lstCustomHelper = new List<CustomHelper>();
                    var productTypeId = string.Empty;
                    foreach (var itemCustomer in itemCustomerGroup)
                    {
                        if (itemCustomer != null)
                        {
                            var customHelper = new CustomHelper
                            {
                                name = itemCustomer.FirstName + " " + itemCustomer.LastName,
                                product_id = returnData.ActivityId,
                                tps_id = string.Empty
                            };
                            if (prioHubProductPaxMapping != null && prioHubProductPaxMapping.Count > 0)
                            {
                                var customerType = ((string)itemCustomer.PassengerType.ToString()).ToUpper();

                                if (!string.IsNullOrEmpty(customerType))
                                {
                                    productTypeId = prioHubProductPaxMapping?.Where(x => x.ProductType?.ToUpper() == customerType).FirstOrDefault()?.ProductTypeId;
                                }
                            }
                            customHelper.tps_id = productTypeId ?? string.Empty;
                            lstCustomHelper.Add(customHelper);
                        }
                    }
                    var customFieldValueJSON = JsonSerializer.Serialize(lstCustomHelper);
                    if (!string.IsNullOrEmpty(customFieldValue.ToString()))
                    {
                        customFieldValue.Append(",");
                    }
                    customFieldValue.Append('"'+""+ productTypeId +'"'+ ":");
                    customFieldValue.Append(customFieldValueJSON);
                    
                }

                var finalCustomFieldData = "{\"" + returnData.ActivityId + "_" + dateString + "\":{" + customFieldValue.ToString() + "}}";

                var orderCustomFields = new Entities.CreateBookingRequest.OrderCustomFields
                {
                    CustomFieldName = "per_participants",
                    CustomFieldValue = finalCustomFieldData
                };
                bookingRq.OrderCustomFields.Add(orderCustomFields);
            }
            
            //end- orderCustomFields

            //start- orderOptions
            var orderOptions = new Entities.CreateBookingRequest.OrderOptions
            {
                EmailOptions = new Entities.CreateBookingRequest.EmailOptions()
            };

            var emailRecipients = new Entities.CreateBookingRequest.EmailRecipients
            {
                RecipientsAddress = returnData?.BookingEmail,
                RecipientsName = returnData?.BookingName
            };
            var emailRecipientsList = new List<Entities.CreateBookingRequest.EmailRecipients>
            {
                emailRecipients
            };

            var emailType = new Entities.CreateBookingRequest.EmailTypes
            {
                SendReceipt = false,
                SendTickets = false
            };

            orderOptions.EmailOptions.EmailRecipients = new List<Entities.CreateBookingRequest.EmailRecipients>();
            orderOptions.EmailOptions.EmailRecipients = emailRecipientsList;

            orderOptions.EmailOptions.EmailTypes = new Entities.CreateBookingRequest.EmailTypes();
            orderOptions.EmailOptions.EmailTypes = emailType;

            bookingRq.OrderOptions = orderOptions;
            //end- orderOptions
            var jsonRequest = SerializeDeSerializeHelper.Serialize(_createBookingRq);
            return jsonRequest;
        }

        protected override object GetResultsAsync(object inputContext)
        {
            var prioHubCriteriaBookingRQ = SerializeDeSerializeHelper.DeSerialize<CreateBookingRequest>(inputContext.ToString());
            var scope = _PrioHubApiScopeBooking;
            var actualDistributerID = prioHubCriteriaBookingRQ?.Data?.Order?.OrderDistributorId;
            //1. Using basic Auth Get AccessToken
            var accessToken = AddRequestHeadersAndAddressToApi(scope, Convert.ToInt32(actualDistributerID));
            //products/2433?distributor_id=501&cache=false

            var actualBaseAddress = _PrioHubServiceURL;
            //if (actualDistributerID == _PrioHubApiDistributorIdPrioOnly)
            //{
            //    actualBaseAddress = _PrioHubServiceURLOnlyPrioProducts;
            //}
            var baseAddress = actualBaseAddress + Constant.Booking;

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
            var returnData = client.PostJsonWithHeadersAsync((CreateBookingRequest)prioHubCriteriaBookingRQ, headers)?.GetAwaiter().GetResult();//client.PostJsonWithHeader((CreateBookingRequest)prioHubCriteriaBookingRQ, headers);

            return returnData;
        }

        ///// <summary>
        ///// Get Results
        ///// </summary>
        ///// <param name="jsonResult"></param>
        ///// <returns></returns>
        //protected override object GetResults(object jsonResult)
        //{
        //    return SerializeDeSerializeHelper.DeSerialize<CreateBookingResponse>(jsonResult.ToString());
        //}
        protected override object GetResponseObject(string responseText)
        {
            return responseText;
        }
    }
}