using Isango.Entities.Activities;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts
{
    public interface IAvailablityConverter : IConverterBase
    {
        ActivityOption ConvertAvailablityResult(object objectResult);
    }
}