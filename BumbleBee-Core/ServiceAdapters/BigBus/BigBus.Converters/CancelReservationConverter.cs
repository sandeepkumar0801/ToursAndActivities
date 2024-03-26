using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.BigBus.BigBus.Converters.Contracts;
using ServiceAdapters.BigBus.BigBus.Entities;
using Util;

namespace ServiceAdapters.BigBus.BigBus.Converters
{
    public class CancelReservationConverter : ConverterBase, ICancelReservationConverter
    {
        public CancelReservationConverter(ILogger logger) : base(logger)
        {
        }

        public override object Convert<T>(string response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<CancelReservationResponse>(response);
            if (result == null) return null;

            var status = new Dictionary<string, bool>();
            var selectedProducts = request as List<SelectedProduct>;

            selectedProducts?.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });

            if (result.CancelReservationResult.Status.Equals(BigBusApiStatus.Cancelled))
            {
                selectedProducts?.ForEach(e => { status[e.AvailabilityReferenceId] = true; });
            }

            return status;
        }
    }
}