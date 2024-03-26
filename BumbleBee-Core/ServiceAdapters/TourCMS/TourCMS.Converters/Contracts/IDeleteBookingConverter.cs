using Isango.Entities.Activities;
using System.Collections.Generic;

namespace ServiceAdapters.TourCMS.TourCMS.Converters.Contracts
{
    public interface IDeleteBookingConverter : IConverterBase
    {
        List<Activity> ConvertAvailablityResult(object optionsFromAPI, object criteria);
    }
}
