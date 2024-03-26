using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Converters.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities.SubmitCheckout;
using ServiceAdapters.Bokun.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Util;
using Booking = Isango.Entities.Booking.Booking;
using System.Linq;
using Barcode = Isango.Entities.Barcode;

namespace ServiceAdapters.Bokun.Bokun.Converters
{
    public class SubmitCheckoutConverter : ConverterBase, ISubmitCheckoutConverter
    {
        public SubmitCheckoutConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">Generic model for Submit Checkout Call</param>
        /// <param name="criteria">Generic request model</param>
        /// <returns></returns>
        public object Convert<T>(T objectResult, T criteria)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<SubmitCheckoutRs>(objectResult.ToString());
            var booking = default(Booking);
            if (result != null)
            {
                booking = ConvertToBookingResult(result);
            }
            return booking;
        }

        private Booking ConvertToBookingResult(SubmitCheckoutRs result)
        {
            var booking = default(Booking);
            try
            {
                var defaultDate = new DateTime(1970, 01, 01, 00, 00, 00);
                decimal.TryParse(result?.Booking?.TotalPrice?.ToString(), out var amount);
                double.TryParse(result?.Booking?.CreationDate?.ToString(), out var date);
                int.TryParse(result?.Booking?.BookingId?.ToString(), out var bookingId);

                booking = new Booking
                {
                    ReferenceNumber = result?.Booking?.ConfirmationCode,
                    BookingId = bookingId,
                    Amount = amount,
                    Currency = new Isango.Entities.Currency
                    {
                        IsoCode = result?.Booking?.Currency
                    },
                    Language = new Language() { Code = result?.Booking?.Language },
                    Date = defaultDate.AddMilliseconds(date),
                    SelectedProducts = new List<SelectedProduct>()
                };

                int tempInt;
                result?.Booking?.ActivityBookings?.ForEach(product =>
                {
                    double.TryParse(product?.StartDateTime?.ToString(), out date);
                    decimal.TryParse(product?.TotalPrice?.ToString(), out amount);
                    var selectedProduct = new BokunSelectedProduct();
                    var activityOption = new ActivityOption();
                    var startDate = defaultDate.AddMilliseconds(date);
                    selectedProduct.Price = amount;

                    int.TryParse(product?.ProductId?.ToString(), out tempInt);
                    selectedProduct.Id = tempInt;
                    selectedProduct.FactsheetId = tempInt;

                    //selectedProduct.Name = product.Title;
                    int.TryParse(product?.RateId?.ToString(), out tempInt);
                    selectedProduct.RateId = tempInt;

                    int.TryParse(product?.ParentBookingId?.ToString(), out tempInt);
                    selectedProduct.ParentBundleId = tempInt;

                    int.TryParse(product?.TotalParticipants?.ToString(), out tempInt);
                    selectedProduct.Quantity = tempInt;

                    selectedProduct.StartTime = System.Convert.ToString(startDate, CultureInfo.InvariantCulture);
                    var barcode = product?.Barcode ?? product.PricingCategoryBookings?.FirstOrDefault(x => x?.Barcode != null)?.Barcode;

                    try
                    {
                        if (product.TicketPerPerson && product.PricingCategoryBookings?.Count > 1)
                        {
                            selectedProduct.Barcodes = new List<Barcode>();
                            foreach (var barcodeField in product.PricingCategoryBookings)
                            {
                                if (!string.IsNullOrWhiteSpace(barcodeField.Barcode?.Value))
                                {
                                    var barcodetemp = new Barcode
                                    {
                                        BarcodeType = barcodeField.Barcode.BarcodeType,
                                        BarCode = barcodeField.Barcode.Value,
                                        PassengerType = barcodeField.BookedTitle,
                                        PricingCategoryId = barcodeField.PricingCategoryId?.ToString()
                                    };
                                    selectedProduct.Barcodes.Add(barcodetemp);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //ignore
                    }
                    if (!string.IsNullOrWhiteSpace(barcode?.Value))
                    {
                        var codeType = barcode?.BarcodeType;
                        if (!string.IsNullOrWhiteSpace(codeType))
                        {
                            codeType += "~";
                        }

                        selectedProduct.QrCode = $"{codeType}{barcode?.Value}";
                    }
                    selectedProduct.ConfirmationCode = product?.ConfirmationCode;
                    selectedProduct.ProductConfirmationCode = product?.ProductConfirmationCode;
                    selectedProduct.SupplierCancellationPolicy = product?.CancellationPolicy == null ? "" : SerializeDeSerializeHelper.Serialize(product.CancellationPolicy);

                    if (product?.ProductCategory == Constant.Activities)
                    {
                        selectedProduct.ProductType = ProductType.Activity;
                    }

                    var price = new Price
                    {
                        DatePriceAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>()
                    };

                    price.DatePriceAndAvailabilty.Add(startDate, new DefaultPriceAndAvailability());
                    activityOption.SellPrice = price;
                    activityOption.Name = product?.Title;
                    activityOption.IsSelected = true;
                    booking.SelectedProducts.Add(selectedProduct);
                });
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                    _logger.Error(new IsangoErrorEntity
                    {
                        ClassName = "Bokun.SubmitCheckoutConverter",
                        MethodName = "ConvertToBookingResult"
                    }, ex)
                );
                //throw; //use throw as existing flow should not break bcoz of logging implementation.
            }
            return booking;
        }
    }
}