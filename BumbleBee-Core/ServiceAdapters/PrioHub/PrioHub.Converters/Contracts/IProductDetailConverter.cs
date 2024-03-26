using Isango.Entities.Activities;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Converters.Contracts
{
    public interface IProductDetailConverter : IConverterBase
    {
        List<ActivityOption> ConvertAvailablityResult(object objectResult);
    }
}