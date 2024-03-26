using Isango.Entities.WirecardPayment;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Converters.Contracts
{
    public interface IConverterBase
    {
        WirecardPaymentResponse Convert(string response, object objResult);
    }
}