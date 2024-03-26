using Isango.Entities;
using Isango.Entities.Redeam;

using ServiceAdapters.Redeam.Redeam.Converters.Contracts;
using ServiceAdapters.Redeam.Redeam.Entities.CreateBooking;

using System.Collections.Generic;
using System.Linq;
using Util;

namespace ServiceAdapters.Redeam.Redeam.Converters
{
    public class CreateBookingConverter : ConverterBase, ICreateBookingConverter
    {
        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override object Convert<T>(T response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<CreateBookingResponse>(response.ToString());
            if (result?.Booking == null)
            {
                throw new System.Exception($"Redeam.Converters failed to DeSerialize response.\n{response?.ToString()}");
            }

            return ConvertBookingResponse(result, request as SelectedProduct);
        }

        #region Private Methods

        private SelectedProduct ConvertBookingResponse(CreateBookingResponse createBookingResponse, SelectedProduct selectedProduct)
        {
            var redeamSelectedProduct = (RedeamSelectedProduct)selectedProduct;
            redeamSelectedProduct.BookingReferenceNumber = createBookingResponse.Booking.Id.ToString();
            redeamSelectedProduct.QrCodes = new List<QrCode>();
            if (createBookingResponse?.Booking?.Tickets != null)
            {
                foreach (var ticket in createBookingResponse.Booking.Tickets)
                {
                    if (ticket.Barcode == null) continue;
                    var qrCode = new QrCode
                    {
                        PassengerType = ticket.Guests.FirstOrDefault().TypeName,
                        Value = ticket.Barcode.Value.ToString(),
                        Type = ticket.Barcode.Format
                    };
                    redeamSelectedProduct.QrCodes.Add(qrCode);
                }
            }
            return selectedProduct;
        }

        #endregion Private Methods
    }
}