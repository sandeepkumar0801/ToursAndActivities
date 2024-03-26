using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using Logger.Contract;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Converters.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels;
using System.Globalization;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Converters
{
    public class BookingConverter : ConverterBase, IBookingConverter
    {
        public BookingConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert<T>(T objectResult, object criteria)
        {
            if (objectResult != null)
            {
                var result = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(objectResult as string);

                if (result?.Booking != null)
                {
                    var bookingList = ConvertBookingResult(result);
                    return bookingList;
                }
            }

            return null;
        }

        /// <summary>
        /// This method maps the API response to isango Contracts objects.
        /// </summary>
        /// <returns>Isango.Contracts.Entities.Supplier Object</returns>
        private object ConvertBookingResult(BookingResponse bookingResponse)
        {
            try
            {
                var bookingObject = bookingResponse.Booking;
                var dateTimeOffset = DateTimeOffset.Parse(bookingObject.Availability.StartAt, null);
                var status = GetOptionBookingStatus(bookingObject);
                var contactPerson = bookingObject.Contact.Name.Split(' ');

                var booking = new Isango.Entities.Booking.Booking
                {
                    SelectedProducts = new List<SelectedProduct>
                    {
                        new FareHarborSelectedProduct
                        {
                            UuId = bookingObject.Uuid,
                            ProductOptions = new List<ProductOption>
                            {
                                new ActivityOption
                                {
                                    Id = bookingObject.Availability.Pk,
                                    Name = $"{GetName(dateTimeOffset.DateTime.ToLongDateString())} {Constant.Tour}",
                                    Customers = new List<Isango.Entities.Customer>
                                    {
                                        new Isango.Entities.Customer
                                        {
                                            IsLeadCustomer = true,
                                            FirstName = contactPerson[0],
                                            LastName = contactPerson[1]
                                        }
                                    },
                                    BookingStatus = status
                                }
                            }
                        }
                    }
                };
                return booking;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "FareHarbor.BookingConverter",
                    MethodName = "ConvertBookingResult"
                };
                _logger.Error(isangoErrorEntity, ex);
                throw; //use throw as existing flow should not break bcoz of logging implementation.
            }
        }

        private string GetName(string name)
        {
            var time = DateTimeOffset.Parse(name, CultureInfo.InvariantCulture).ToString(Constant.DateTimeOffsetFormat);

            return time;
        }

        private OptionBookingStatus GetOptionBookingStatus(CreateBookingResponse bookingObject)
        {
            if (bookingObject.Status.ToLowerInvariant() == "cancelled")
            {
                return OptionBookingStatus.Cancelled;
            }

            return OptionBookingStatus.Confirmed;
        }
    }
}