using Isango.Entities;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts
{
    public interface IUpdateBookingConverter : IConverterBase
    {
        SelectedProduct ConvertUpdateBooking(object result);
    }
}