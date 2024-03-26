using Isango.Entities;
using Isango.Entities.Rezdy;

using ServiceAdapters.Rezdy.Rezdy.Converters.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities.Booking;
using System;
using System.Collections.Generic;
using System.Linq;

using Util;

namespace ServiceAdapters.Rezdy.Rezdy.Converters
{
    public class CreateBookingConverter : ConverterBase, ICreateBookingConverter
    {
        public override object Convert<T>(string response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(response.ToString());
            if (result == null || result.RequestStatus.Success == "false") return null;
            return ConvertbookingResult(result, request as List<SelectedProduct>);
        }
        public override object Convert<T, A>(string response, T request, A apiResponse)
        {
            return null;
        }
        public object ConvertbookingResult(BookingResponse bookingResponse, List<SelectedProduct> selectedProducts)
        {
            selectedProducts.ForEach(x => ((RezdySelectedProduct)x).OrderNumber = bookingResponse.Booking.OrderNumber);

            foreach (var item in bookingResponse.Booking.Items)
            {
                var selectedProduct = (RezdySelectedProduct)selectedProducts.FirstOrDefault(x => ((RezdySelectedProduct)x).ProductCode == item.ProductCode);
                if (selectedProduct != null)
                {
                    selectedProduct.Barcodes = new List<Barcode>();
                    //if Participants are not blank
                    if (item?.Participants != null && item?.Participants?.Length > 0)
                    {
                        foreach (var participant in item.Participants)
                        {
                            var barcodefield = participant.Fields.AsEnumerable().FirstOrDefault(x => x.Label == "Barcode");
                            var firstNameField = new Field();
                            var lastNameField = new Field();
                            try
                            {
                                firstNameField = participant.Fields.AsEnumerable().FirstOrDefault(x => x.Label == "First Name");
                                lastNameField = participant.Fields.AsEnumerable().FirstOrDefault(x => x.Label == "Last Name");
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }
                            if (barcodefield != null)
                            {
                                var barcode = new Barcode
                                {
                                    BarCode = barcodefield.Value,
                                    FirstName = firstNameField?.Value,
                                    LastName = lastNameField?.Value
                                };
                                selectedProduct.Barcodes.Add(barcode);
                            }
                        }
                    }
                    else//if Participants are blank
                    {
                        var bookingFieldNode = bookingResponse.Booking.Fields;
                        foreach (var field in bookingFieldNode)
                        {
                            var barcodefield = (field.Label == "Barcode");
                            if (barcodefield == true)
                            {
                                var barcode = new Barcode
                                {
                                    BarCode = field.Value
                                };
                                selectedProduct.Barcodes.Add(barcode);
                            }
                        }
                    }

                    if (item.PickupLocation != null)
                    {
                        selectedProduct.HotelPickUpLocation =
                            $"{item.PickupLocation.LocationName} {item.PickupLocation.Address} @ {item.PickupLocation.PickupTime}";
                    }
                }
            }





            return selectedProducts;
        }
    }
}