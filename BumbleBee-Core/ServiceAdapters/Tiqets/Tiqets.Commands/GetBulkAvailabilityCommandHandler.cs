using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
    public class GetBulkAvailabilityCommandHandler : CommandHandlerBase, IGetBulkAvailabilityCommandHandler
    {
        private readonly ILogger _log;

        public GetBulkAvailabilityCommandHandler(ILogger log) : base(log)
        {
            _log = log;
        }

        protected override object TiqetsApiRequest<T>(T inputContext)
        {
            var availabilityCriteria = inputContext as TiqetsCriteria;
            using (var httpClient = AddRequestHeadersAndAddressToApi(availabilityCriteria?.AffiliateId))
            {
                var result = httpClient.GetAsync(FormUrlAvailability(availabilityCriteria));
                try
                {
                    result.Wait();
                    return result.Result;
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                    {
                        ClassName = "GetBulkAvailabilityCommandHandler",
                        MethodName = "TiqetsApiRequest",
                        Params = Util.SerializeDeSerializeHelper.Serialize(inputContext)
                    };
                    _log.Error(isangoErrorEntity, ex);
                    //timeout probably - check logs;
                    return null;
                }
            }
        }

        private string FormUrlAvailability(TiqetsCriteria availabilityCriteria)
        {
            var url = $"{UriConstant.Products}{availabilityCriteria.ProductId}{UriConstant.BulkAvailability}";
            if (availabilityCriteria.CheckoutDate != DateTime.MinValue && availabilityCriteria.CheckinDate == DateTime.MinValue)
            {
                url = $"{url}{UriConstant.BulkAvailabilityWithEndDate}{availabilityCriteria.CheckoutDate.ToString("yyyy-MM-dd")}";
            }
            else if (availabilityCriteria.CheckoutDate != DateTime.MinValue && availabilityCriteria.CheckinDate != DateTime.MinValue)
            {
                url = $"{url}{UriConstant.BulkAvailabilityWithEndDate}{availabilityCriteria.CheckoutDate.ToString("yyyy-MM-dd")}{UriConstant.BulkAvailabilityWithStartDate}{availabilityCriteria.CheckinDate.ToString("yyyy-MM-dd")}";
            }

            return url;
        }
    }
}