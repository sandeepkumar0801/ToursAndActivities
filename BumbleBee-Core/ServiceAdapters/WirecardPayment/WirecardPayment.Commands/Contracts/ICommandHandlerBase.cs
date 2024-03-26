using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System;
using System.Threading.Tasks;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts
{
    public interface ICommandHandlerBase
    {
        Tuple<string, string> Execute(PaymentCardCriteria paymentCardCriteria, bool is3D, string token);

        Task<string> ExecuteAsync(PaymentCardCriteria paymentCardCriteria, bool is3D, string token);
    }
}