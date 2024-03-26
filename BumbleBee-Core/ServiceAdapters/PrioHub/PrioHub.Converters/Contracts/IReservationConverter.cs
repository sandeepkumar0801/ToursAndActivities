using System;

namespace ServiceAdapters.PrioHub.PrioHub.Converters.Contracts
{
    public interface IReservationConverter : IConverterBase
    {
        Tuple<string, string, string,string> ConvertReservationResult(object objectResult);
    }
}