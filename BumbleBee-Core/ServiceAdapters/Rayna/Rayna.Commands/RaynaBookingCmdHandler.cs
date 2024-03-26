using Isango.Entities.Activities;
using Isango.Entities.Rayna;
using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaBookingCmdHandler : CommandHandlerBase, IRaynaBookingCmdHandler
    {
        private string _supportEmail = "support@isango.com";

        public RaynaBookingCmdHandler(ILogger iLog) : base(iLog)
        {
            try
            {
                _supportEmail = ConfigurationManagerHelper.GetValuefromAppSettings("mailfrom");
            }
            catch(Exception ex)
            {
                //ignore
            }
        }

        protected override object CreateInputRequest<T>(T bookingContext)
        {

            var inputContext = bookingContext as ServiceAdapters.Rayna.Rayna.Entities.InputContext;
            var selectedProducts = inputContext?.SelectedProductsLst;
            var voucherPhoneNumber = inputContext?.VoucherPhoneNumber;
            var isangoRefNumber = inputContext?.BookingReference;

            Int64.TryParse(voucherPhoneNumber, out long vhrPhoneNumber);
            var returnData = new BookingREQ
            {
                UniqueNo = "",
                Passengers = new List<Passenger>(),
                TourDetails = new List<Tourdetail>(),
            };



            var passengerList = new List<Passenger>();
            var tourDetailList = new List<Tourdetail>();
            //loop start for TourDetails-Repeative Data based on multiple products
            foreach (var singleselectedProduct in selectedProducts)
            {
                var raynaSelectdProducts = (RaynaSelectedProduct)singleselectedProduct;
                if (!(raynaSelectdProducts != null))
                {
                    return null;
                }
                var selectedProduct = raynaSelectdProducts;

                var selectedOptions = selectedProduct?.ProductOptions
                    ?.FindAll(f => f.IsSelected.Equals(true))
                    ?.Cast<ActivityOption>().ToList();

                ///////***************Passenger Start -common ************************/////////

                var selectedOptionsGet = selectedOptions?.FirstOrDefault();
                var lstCustomers = selectedOptionsGet?.Customers;
                returnData.UniqueNo = isangoRefNumber;
                //You can pass only one lead guest name in one booking reference, 
                //even if there are multiple products under the same booking.
                if (passengerList == null || passengerList.Count == 0)
                {
                    if (lstCustomers != null && lstCustomers.Count > 0)
                    {
                        foreach (var customerData in lstCustomers)
                        {
                            if (customerData.IsLeadCustomer == true)
                            {
                                var passenger = new Passenger
                                {
                                    ClientReferenceNo = isangoRefNumber,
                                    Email = _supportEmail ?? customerData.Email,
                                    FirstName = customerData?.FirstName,
                                    LastName = customerData?.LastName,
                                    LeadPassenger = customerData.IsLeadCustomer == true ? 1 : 0,
                                    Message = "",//Short information about the passenger.
                                    Mobile = Convert.ToString(vhrPhoneNumber),
                                    Nationality = "",//Passanger's nationality.
                                    PaxType = Convert.ToString(customerData.PassengerType),//The passanger is Adult, Chind or Infant.
                                    Prefix = String.IsNullOrEmpty(customerData?.Title) ? "Mr." : customerData?.Title,
                                    //Mr./ Ms./ Mrs.
                                    ServiceType = "tour" //Service type is tour.
                                };
                                passengerList.Add(passenger);
                            }
                        }
                    }
                }

                ///////***************Passenger End************************/////////


                ///////***************Tour Start************************/////////
                if (selectedOptionsGet != null)
                {
                    var adultCount = 0;
                    var childCount = 0;
                    var infantCount = 0;

                    if (lstCustomers != null && lstCustomers?.Count > 0)
                    {
                        var adultCountData = lstCustomers?.Where(x => x.PassengerType.ToString() == Isango.Entities.Enums.PassengerType.Adult.ToString())?.ToList();
                        if (adultCountData != null && adultCountData.Count > 0)
                        {
                            adultCount = adultCountData.Count;
                        }

                        var childCountData = lstCustomers?.Where(x => x.PassengerType.ToString() == Isango.Entities.Enums.PassengerType.Child.ToString())?.ToList();
                        if (childCountData != null && childCountData.Count > 0)
                        {
                            childCount = childCountData.Count;
                        }

                        var infantCountData = lstCustomers?.Where(x => x.PassengerType.ToString() == Isango.Entities.Enums.PassengerType.Infant.ToString())?.ToList();
                        if (infantCountData != null && infantCountData.Count > 0)
                        {
                            infantCount = infantCountData.Count;
                        }
                    }
                    var startDate = selectedOptionsGet?.TravelInfo?.StartDate;
                    var currentDatePriceAndAvailability = selectedOptionsGet?.CostPrice?.DatePriceAndAvailabilty?.Where(x => x.Key.ToString() == startDate.ToString())?.FirstOrDefault().Value;
                    var totalAmount = currentDatePriceAndAvailability?.PricingUnits?.Sum(x => x.Price);

                    var adultPrice = 0m;
                    var childPrice = 0m;
                    var pricingUnitsPerPerson = currentDatePriceAndAvailability.PerPersonPricingUnit;
                    if (pricingUnitsPerPerson != null && pricingUnitsPerPerson.Count > 0)
                    {
                        foreach (var item in pricingUnitsPerPerson)
                        {
                            if (item.PassengerType == Isango.Entities.Enums.PassengerType.Adult)
                            {
                                adultPrice = item.Price * item.Quantity;
                            }
                            else if (item.PassengerType == Isango.Entities.Enums.PassengerType.Child)
                            {
                                childPrice = item.Price * item.Quantity;
                            }
                        }
                    }

                    var pickupLocation = string.Empty;
                    if (selectedOptionsGet.TransferId == 41843.ToString() || selectedOptionsGet.TransferId == 41844.ToString() ||
                        selectedOptionsGet.TransferId == 43129.ToString())
                    {
                        pickupLocation = selectedOptionsGet?.HotelPickUpLocation;
                        if (String.IsNullOrEmpty(pickupLocation))
                        {
                            pickupLocation = "no pickup enter by the user";
                        }
                    }

                    var tourDetail = new Tourdetail
                    {
                        Adult = adultCount,  //Total number of adult.
                        AdultRate = adultPrice,
                        Child = childCount,  //Total number of child.
                        ChildRate = childPrice,//Total child price.
                        Infant = infantCount,//Total number of infant.

                        TourId = Convert.ToInt32(selectedOptionsGet.TourId),//Unique identifier of the tour, You will get tourId from tourlist API.
                        OptionId = Convert.ToInt32(selectedOptionsGet.TourOptionId),//You will get tourOptionId from touroption API.
                        TransferId = Convert.ToInt32(selectedOptionsGet.TransferId),
                        //You should enter the transferId as per requierment.
                        //1   Without Transfer    41865
                        //2   Sharing Transfer    41843
                        //3   Private Transfer    41844
                        //4   Private Boat Without Transfers  43129
                        //5   Pvt Yach Without Transfer   43110
                        TimeSlotId = Convert.ToInt32(selectedOptionsGet?.TimeSlotId),//Unique identifier of the tour options time slot. (Note: If required)
                        StartTime = Convert.ToString(selectedOptionsGet?.TourStartTime),//Start timing of tour.

                        Pickup = pickupLocation,//Enter pickup location. (If requier)
                                                //Always greater than or equal to 6 digits(ServiceUniqueId)
                        ServiceUniqueId =
                        (Math.Floor(Math.Log10(selectedOptionsGet.ServiceOptionId) + 1)) >= 6 ? Convert.ToString(selectedOptionsGet.ServiceOptionId) : Convert.ToString(selectedOptionsGet.ServiceOptionId.ToString("D6")),
                        //Service uniqueId must be unique for each tour booked under the sam referance. If the referanceNo having only 
                        //one tour then you can pass same uniqueId. (Note: Max 6 digit)
                        TourDate = Convert.ToString(startDate)//Tour booking date Formats.
                    };
                    tourDetail.ServiceTotal = Convert.ToString(tourDetail?.AdultRate + tourDetail?.ChildRate);
                    tourDetailList.Add(tourDetail);
                }
                ///////***************Tour End************************/////////


            }

            returnData.Passengers = passengerList;
            returnData.TourDetails = tourDetailList;
            return returnData;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(BookingRES);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<BookingRES>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override object GetResults(object input)
        {
            var data = input as BookingREQ;
            var url = string.Format($"{_raynaURL}{Constants.Constants.Booking}");
            var response = GetResponseFromAPIEndPoint(data, url);
            return response;
        }
    }
}