using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.TourCMS;
using Logger.Contract;
using ServiceAdapters.TourCMS.TourCMS.Converters.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities;
using ServiceAdapters.TourCMS.TourCMS.Entities.CommitBooking;
using Util;

namespace ServiceAdapters.TourCMS.TourCMS.Converters
{
    public class CommitBookingConverter : ConverterBase, ICommitBookingConverter
    {

        public CommitBookingConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>
        public override object Convert<T>(string response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerializeXml<CommitBookingResponse>(response);
            if (result == null) return null;

            return ConvertBookingResult(result, request as SelectedProduct);
        }

        private SelectedProduct ConvertBookingResult(CommitBookingResponse bookingResponse, SelectedProduct selectedProduct)
        {
            try
            {
                var CheckComponents = bookingResponse?.booking?.components;
                if (CheckComponents == null) return null;

                var tourCMSSelectedProduct = (TourCMSSelectedProduct)selectedProduct;
                tourCMSSelectedProduct.BookingStatus = System.Convert.ToString(bookingResponse.booking.Status);
                tourCMSSelectedProduct.BookingReference = System.Convert.ToString(bookingResponse.booking.BookingId);
                tourCMSSelectedProduct.ShortReference = System.Convert.ToString(bookingResponse.booking.BookingId);
                tourCMSSelectedProduct.SupplierReferenceNumber = System.Convert.ToString(bookingResponse.booking.BookingId);
                var barcodeData = bookingResponse?.booking?.BarcodeData;
                tourCMSSelectedProduct.TourCMSTicket = new List<TourCMSTicket>();
                foreach (var item in bookingResponse?.booking?.components?.component)
                {
                    var passengerType = item.RateDescription;
                    var tourCMSTicket = new TourCMSTicket
                    {
                        TicketType = item.ProductCode,
                        PassengerType = passengerType,
                        Quantity = System.Convert.ToString(item.SaleQuantity)
                    };

                    var ticket = item?.Tickets?.Ticket;
                    //Per Item PerComponent
                    if (ticket != null)
                    {
                        
                        foreach (var itemTicket in ticket)
                        {
                            var itemTour = new TourCMSTicket
                            {
                                TicketType = item.ProductCode,
                                PassengerType = passengerType,
                                BarcodeSymbology= item?.BarcodeSymbology,// QR code or Barcode
                                Quantity = System.Convert.ToString(item.SaleQuantity),
                            };
                            if (String.IsNullOrEmpty(item.OperatorReference))
                            {
                                itemTour.TicketBarCode = itemTicket?.Value + "!!!" + barcodeData;
                            }
                            else
                            {
                                itemTour.TicketBarCode = itemTicket?.Value + "!!!" + item.OperatorReference;
                            }
                            tourCMSSelectedProduct.TourCMSTicket.Add(itemTour);
                        }
                    }
                    else //One Item Per Component
                    {
                        if (String.IsNullOrEmpty(item.OperatorReference))
                        {
                            tourCMSTicket.TicketBarCode = barcodeData;
                            tourCMSTicket.BarcodeSymbology = item?.BarcodeSymbology;// QR code or Barcode
                        }
                        else
                        {
                            tourCMSTicket.TicketBarCode = item.OperatorReference + "!!!" + barcodeData;
                            tourCMSTicket.BarcodeSymbology = item?.BarcodeSymbology;// QR code or Barcode
                        }
                        tourCMSSelectedProduct.TourCMSTicket.Add(tourCMSTicket);
                        //tourCMSSelectedProduct.TourCMSTicket = new List<TourCMSTicket>
                        //{
                            //tourCMSTicket
                        //};
                    }
                   
                }

                tourCMSSelectedProduct.BarcodedData = bookingResponse.booking.BarcodeData;


                return selectedProduct;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TourCMS.CreateBookingConverter",
                    MethodName = "ConvertBookingResult"
                };
                _logger.Error(isangoErrorEntity, ex);
                throw; //use throw as existing flow should not break bcoz of logging implementation.
            }
        }

        public override object Convert(object objectResponse, object criteria)
        {
            throw new NotImplementedException();
        }

        List<Activity> ICommitBookingConverter.ConvertAvailablityResult(object optionsFromAPI, object criteria)
        {
            throw new NotImplementedException();
        }

        object IConverterBase.Convert(object apiResponse, MethodType methodType, object criteria)
        {
            throw new NotImplementedException();
        }
        public object Convert(object apiResponse, ServiceAdapters.TourCMS.TourCMS.Entities.MethodType methodType, object criteria = null)
        {
            throw new NotImplementedException();
        }

        object IConverterBase.Convert(object objectResult, object criteria)
        {
            throw new NotImplementedException();
        }
    }
}