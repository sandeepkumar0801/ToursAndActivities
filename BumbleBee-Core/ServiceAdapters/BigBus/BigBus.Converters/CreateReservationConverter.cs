using Isango.Entities;
using Isango.Entities.BigBus;
using Logger.Contract;
using ServiceAdapters.BigBus.BigBus.Converters.Contracts;
using ServiceAdapters.BigBus.BigBus.Entities;
using Util;

namespace ServiceAdapters.BigBus.BigBus.Converters
{
    public class CreateReservationConverter : ConverterBase, ICreateReservationConverter
    {
        public CreateReservationConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert<T>(string response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<ReservationResponse>(response);
            if (result == null) return null;

            return ConvertReserevationResult(result, request as List<SelectedProduct>);
        }

        private List<SelectedProduct> ConvertReserevationResult(ReservationResponse reservationResponse, List<SelectedProduct> selectedProducts)
        {
            if (reservationResponse.Status == BigBusApiStatus.Reserved)
            {
                selectedProducts.ForEach(x =>
                    {
                        ((BigBusSelectedProduct)x).ReservationReference = reservationResponse.ReservationResult.ReservationReference;
                        ((BigBusSelectedProduct)x).BookingStatus = reservationResponse.ReservationResult.Status;
                    });
            }
            else
            {
                selectedProducts.ForEach(x =>
                {
                    ((BigBusSelectedProduct)x).BookingStatus = reservationResponse.ReservationResult.Status;
                });
            }

            return selectedProducts;
        }
    }
}