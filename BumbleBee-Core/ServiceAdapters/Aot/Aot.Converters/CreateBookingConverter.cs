using Isango.Entities;
using Isango.Entities.Aot;
using Isango.Entities.Booking;
using Logger.Contract;
using ServiceAdapters.Aot.Aot.Converters.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;

namespace ServiceAdapters.Aot.Aot.Converters
{
    public class CreateBookingConverter : ConverterBase, ICreateBookingConverter
    {
        public CreateBookingConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectresult">string response Add Booking Request call</param>
        /// <param name="inputRequest"></param>
        /// <returns></returns>
        public override object Convert<T>(T objectresult, T inputRequest)
        {
            var result = DeSerializeXml<AddBookingResponse>(objectresult as string);
            var convertedResult = ConvertBookingDetailsResult(result, inputRequest as List<SelectedProduct>);
            return convertedResult;
        }

        /// <summary>
        ///Conver Booking Detais Result
        /// </summary>

        /// <returns></returns>
        private object ConvertBookingDetailsResult(AddBookingResponse bookingResponse, List<SelectedProduct> inputRequest)
        {
            var booking = new Booking();
            if (bookingResponse != null)
            {
                booking.ReferenceNumber = bookingResponse.Ref;
                booking.SelectedProducts = new List<SelectedProduct>();
                var addServiceResponseDetails = bookingResponse.AddServiceResponses.AddServiceResponse.ToList();

                for (var i = 0; i < addServiceResponseDetails.Count; i++)
                {
                    try
                    {
                        var selectedProduct = inputRequest[i] as AotSelectedProduct;
                        if (selectedProduct == null) continue;
                        selectedProduct.ServiceLineId = addServiceResponseDetails[i].ServiceLineId;
                        selectedProduct.Status = addServiceResponseDetails[i].Status;
                        selectedProduct.SequenceNumber = addServiceResponseDetails[i].SequenceNumber;
                        booking.SelectedProducts.Add(selectedProduct);
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "Aot.CreateBookingConverter",
                            MethodName = "ConvertBookingDetailsResult"
                        };
                        _logger.Error(isangoErrorEntity, ex);
                        throw; //use throw as existing flow should not break bcoz of logging implementation.
                    }
                }
            }
            return booking;
        }
    }
}