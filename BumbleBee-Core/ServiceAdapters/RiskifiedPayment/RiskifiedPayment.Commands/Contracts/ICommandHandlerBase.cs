using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities;
using System.Threading.Tasks;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        Task<object> ExecuteAsync(object inputContext, object requestExtraData, string endPoint, string token, MethodType methodType);

        object Execute(object inputContext, object requestExtraData, string endPoint, string token, MethodType methodType);
    }
}