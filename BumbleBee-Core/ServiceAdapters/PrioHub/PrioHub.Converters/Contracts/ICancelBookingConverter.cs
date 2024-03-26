using System;

namespace ServiceAdapters.PrioHub.PrioHub.Converters.Contracts
{
    public interface ICancelBookingConverter : IConverterBase
    {
        Tuple<string, string, string,string,string, DateTime> ConvertCancelResult(object objectResult);
    }
}