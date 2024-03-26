using System;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts
{
    public interface ICancelBookingConverter : IConverterBase
    {
        Tuple<string, string, string, DateTime> ConvertCancelResult(object objectResult);
    }
}