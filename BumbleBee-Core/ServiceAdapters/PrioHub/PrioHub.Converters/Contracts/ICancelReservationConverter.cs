using System;

namespace ServiceAdapters.PrioHub.PrioHub.Converters.Contracts
{
    public interface ICancelReservationConverter : IConverterBase
    {
        Tuple<string, string, string, DateTime> ConvertCancelResult(object objectResult);
    }
}