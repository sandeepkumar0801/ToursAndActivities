using Isango.Entities.Activities;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Converters.Contracts
{
    public interface IAvailablityListConverter : IConverterBase
    {
        List<ActivityOption> ConvertAvailablityResult(object objectResult);
    }
}