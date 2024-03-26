using System;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts
{
    public interface IReservationConverter : IConverterBase
    {
        Tuple<string, string, string> ConvertReservationResult(object objectResult);
    }
}