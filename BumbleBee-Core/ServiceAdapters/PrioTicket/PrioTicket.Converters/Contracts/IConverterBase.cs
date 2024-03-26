using ServiceAdapters.PrioTicket.PrioTicket.Entities;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts
{
    public interface IConverterBase
    {
        MethodType Converter { get; set; }

        object Convert(object objectResult);
    }
}