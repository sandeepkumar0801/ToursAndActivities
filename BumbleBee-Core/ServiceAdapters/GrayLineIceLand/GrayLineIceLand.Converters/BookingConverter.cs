using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.Enums;
using Isango.Entities.GrayLineIceLand;
using Logger.Contract;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Converters.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Converters
{
    public class BookingConverter : ConverterBase, IBookingConverter
    {
        public BookingConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert(object objectResult)
        {
            try
            {
                var bookingResponse = (BookingRS)objectResult;
                var booking = new Booking
                {
                    ReferenceNumber = bookingResponse.BookingNumber.ToString(),
                    Date = bookingResponse.Created,
                    Currency = new Currency
                    {
                        IsoCode = bookingResponse.CurrencyCode
                    },
                    SelectedProducts = new List<SelectedProduct>()
                };
                var groupedOrderDetails = bookingResponse.OrderDetails.GroupBy(x => x.TourDepartureId);
                var customers = new List<Customer>();

                foreach (var product in groupedOrderDetails)
                {
                    var selectedPaxMapping = bookingResponse?.SelectedProduct?.Where(x => x.Code.ToLowerInvariant() == product.FirstOrDefault()?.TourNumber.ToLowerInvariant()).FirstOrDefault();

                    var adultPaxMappingId = System.Convert.ToInt32((selectedPaxMapping?.PaxAgeGroupIds.FirstOrDefault(x => x.Key == PassengerType.Adult))?.Value);
                    var childPaxMappingId = System.Convert.ToInt32((selectedPaxMapping?.PaxAgeGroupIds.FirstOrDefault(x => x.Key == PassengerType.Child))?.Value);
                    var youthPaxMappingId = System.Convert.ToInt32((selectedPaxMapping?.PaxAgeGroupIds.FirstOrDefault(x => x.Key == PassengerType.Youth))?.Value);

                    var selectedProduct = new GrayLineIceLandSelectedProduct();
                    var first = product.FirstOrDefault();

                    if (first != null) selectedProduct.ReservationId = first.TourDepartureId;
                    selectedProduct.Code = product.FirstOrDefault()?.TourNumber;
                    selectedProduct.Price = product.FirstOrDefault()?.BasePrice ?? 0;
                    selectedProduct.HotelPickUpLocation = product.FirstOrDefault()?.PickupLocationId.ToString();
                    selectedProduct.HotelPickup = product.FirstOrDefault()?.PickupPlaceText;
                    selectedProduct.ProductOptions = new List<ProductOption>();

                    customers.Add(new Customer()
                    {
                        FirstName = bookingResponse.FirstName,
                        LastName = bookingResponse.LastName,
                        Email = bookingResponse.Email,
                        IsLeadCustomer = true,
                        PassengerType = PassengerType.Adult
                    });

                    var activityOption = new ActivityOption();

                    var adultPrice = new AdultPricingUnit
                    {
                        Price = bookingResponse.OrderDetails.FirstOrDefault(x => x.AgeGroup == adultPaxMappingId)?.BasePrice ?? 0
                    };

                    var youthPrice = new YouthPricingUnit
                    {
                        Price = bookingResponse.OrderDetails.Where(x => x.AgeGroup == youthPaxMappingId)?.FirstOrDefault() != null ?
                        bookingResponse.OrderDetails.FirstOrDefault(x => x.AgeGroup == youthPaxMappingId)?.BasePrice ?? 0 : 0,
                    };

                    var childPrice = new ChildPricingUnit
                    {
                        Price = bookingResponse.OrderDetails.Where(x => x.AgeGroup == childPaxMappingId)?.FirstOrDefault() != null ?
                        bookingResponse.OrderDetails.FirstOrDefault(x => x.AgeGroup == childPaxMappingId)?.BasePrice ?? 0 : 0,
                    };

                    var pricingUnitList = new List<PricingUnit> { adultPrice, youthPrice, childPrice };
                    var groupedProduct = product.FirstOrDefault() ?? new Orderdetail();
                    var price = new Isango.Entities.Price
                    {
                        DatePriceAndAvailabilty =
                            new Dictionary<System.DateTime, PriceAndAvailability>
                            {
                            {
                                groupedProduct.Departure, new DefaultPriceAndAvailability()
                                {
                                    PricingUnits = pricingUnitList,
                                    TotalPrice = product.Sum(x => (x.BasePrice * x.Quantity)),
                                    TourDepartureId = groupedProduct.TourDepartureId
                                }
                            }
                            }
                    };

                    activityOption.CostPrice = price;
                    activityOption.Name = product.FirstOrDefault()?.OrderDetailDescription;
                    selectedProduct.Name = product.FirstOrDefault()?.OrderDetailDescription;
                    activityOption.Customers = customers;
                    activityOption.IsSelected = true;

                    selectedProduct.ProductOptions.Add(activityOption);
                    booking.SelectedProducts.Add(selectedProduct);
                }
                return booking;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GrayLineIceLand.BookingConverter",
                    MethodName = "Convert"
                };
                _logger.Error(isangoErrorEntity, ex);
                throw; //use throw as existing flow should not break bcoz of logging implementation.
            }
        }

        public override object Convert(object objectResult, object input)
        {
            throw new System.NotImplementedException();
        }
    }
}