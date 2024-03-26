using Isango.Entities.Activities;
using Isango.Entities.NewCitySightSeeing;
using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing.Constants;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands.Contract;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Reservation;
using System.Text;
using Util;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands
{
    public class ReservationCmdHandler : CommandHandlerBase, IReservationCommandHandler
    {
        public ReservationCmdHandler(ILogger iLog) : base(iLog)
        {
        }

       
        protected override object CreateInputRequest<T>(T bookingContext)
        {
            var reservationRequest = new ReservationRequest();
            try
            {
                var inputContext = bookingContext as ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.InputContext;
                var selectedProducts = inputContext.SelectedProducts;
                var languageCode = inputContext.LanguageCode;
                var voucherEmailAddress = inputContext.VoucherEmailAddress;
                var VoucherPhoneNumber = inputContext.VoucherPhoneNumber;
                var TotalCustomers = inputContext.TotalCustomers;
                var IsangoRefNumber = inputContext?.BookingReference;

                var postCode = inputContext?.PostCode;
                var address = inputContext?.Address;
                var city = inputContext?.City;

                var newCitySightSeeingSelectdProducts = (NewCitySightSeeingSelectedProduct)selectedProducts;
                if (!(newCitySightSeeingSelectdProducts != null))
                {
                    return null;
                }
                var selectedProduct = newCitySightSeeingSelectdProducts;

                var selectedOptions = selectedProduct?.ProductOptions
                   ?.FindAll(f => f.IsSelected.Equals(true))
                   ?.Cast<ActivityOption>().ToList();

                var leadCustomer = selectedProduct?.ProductOptions?
                   .Find(f => f.IsSelected.Equals(true))
                   .Customers.Find(f => f.IsLeadCustomer.Equals(true));

                 var customerName = leadCustomer?.FirstName.Contains(" ") == true ?
                    leadCustomer?.FirstName?.Split(' ')?.ToList() :
                    new List<string> { { leadCustomer.FirstName }, { leadCustomer.LastName } };

                var lName = customerName.LastOrDefault() ?? "NA";
                var fName = customerName.FirstOrDefault() ?? "NA";

                //Pending  Name Surname Strategy either in booking or Customer
                var contact = new Contact
                {
                    Name = fName,
                    Surname = lName,
                    Email = voucherEmailAddress,
                    Phone = VoucherPhoneNumber,
                    Address = address,
                    City= city,
                    FiscalCode="",
                    Province="",
                    Zip= postCode
                };
                
                var travelInfoList = selectedOptions?.FirstOrDefault()?.TravelInfo;
                var linesList = new List<Line>();

                if (travelInfoList != null)
                {
                    foreach (var travelInfo in travelInfoList?.NoOfPassengers)
                    {
                        var lines = new Line
                        {
                            Rate = Convert.ToString(travelInfo.Key),
                            Quantity = travelInfo.Value
                        };
                        linesList.Add(lines);
                    }
                }
                //Reservation Request
                var rateKey = selectedOptions?.FirstOrDefault()?.RateKey;
                var variationCondition = selectedOptions?.FirstOrDefault()?.VariantCondition;
                var rateCode = selectedOptions?.FirstOrDefault()?.RateCode;
                
                reservationRequest.ExternalServiceRefCode = IsangoRefNumber;
                reservationRequest.ProductCode = rateCode;
                if (variationCondition == true)
                {
                    reservationRequest.VariantCode = rateKey;
                }
                else
                {
                    reservationRequest.VariantCode = string.Empty;
                }
                reservationRequest.ReservationDate = travelInfoList.StartDate;
                reservationRequest.Contact = contact;
                reservationRequest.Lines = linesList;
            }
            catch (Exception)
            {
                throw;
            }
            return reservationRequest;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(ReservationResponse);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<ReservationResponse>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override  object GetResultsAsync(object input)
        {
            var url = $"{_newCitySightSeeingServiceURL}{Constant.Reservation}";
            var reservationRQ = input as ReservationRequest;
            if (reservationRQ == null)
            {
                return null;
            }
            var client = new AsyncClient
            {
                ServiceURL = url
            };
            return client.ConsumePostService(GetHttpRequestHeaders(), Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(reservationRQ), Encoding.UTF8);
        }

    }
}