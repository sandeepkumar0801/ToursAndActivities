using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
    public class GetAvailableDaysCommandHandler : CommandHandlerBase, IGetAvailableDaysCommandHandler
    {
        public GetAvailableDaysCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object TiqetsApiRequest<T>(T inputContext)
        {
            var availabilityCriteria = inputContext as TiqetsCriteria;
            using (var httpClient = AddRequestHeadersAndAddressToApi(availabilityCriteria?.AffiliateId))
            {
                var result = httpClient.GetAsync(FormUrlAvailability(availabilityCriteria));
                result.Wait();
                return result.Result;
            }
        }

        private string FormUrlAvailability(TiqetsCriteria availabilityCriteria)
        {
            return $"{UriConstant.Products}{availabilityCriteria.ProductId}{UriConstant.AvailableDays}{availabilityCriteria.Language}";
        }
    }
}