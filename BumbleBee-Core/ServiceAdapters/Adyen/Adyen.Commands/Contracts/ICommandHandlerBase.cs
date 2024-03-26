using ServiceAdapters.Adyen.Adyen.Entities;

namespace ServiceAdapters.Adyen.Adyen.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        string Execute(AdyenCriteria adyenCriteria, string token, int adyenMerchantType = 1);
    }
}