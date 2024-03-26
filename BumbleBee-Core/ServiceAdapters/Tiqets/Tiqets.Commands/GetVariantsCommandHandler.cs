using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
    public class GetVariantsCommandHandler : CommandHandlerBase, IGetVariantsCommandHandler
    {
        public GetVariantsCommandHandler(ILogger log) : base(log)
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

            //#region ### Read Static file to avoid actual booking

            //string responseResult = string.Empty;
            //using (StreamReader r = new StreamReader(@"D:\temp\29121_Variant.json"))
            //{
            //    responseResult = r.ReadToEnd();
            //}
            //return responseResult;

            //#endregion ### Read Static file to avoid actual booking
        }

        private string FormUrlAvailability(TiqetsCriteria availabilityCriteria)
        {
            return availabilityCriteria.TimeSlot != null ? $"{UriConstant.Products}{availabilityCriteria.ProductId}{UriConstant.Variants}{availabilityCriteria.Language}{UriConstant.Day}{availabilityCriteria.CheckinDate:yyyy-MM-dd}{UriConstant.TimeSlot}{availabilityCriteria.TimeSlot}" : $"{UriConstant.Products}{availabilityCriteria.ProductId}{UriConstant.Variants}{availabilityCriteria.Language}{UriConstant.Day}{availabilityCriteria.CheckinDate:yyyy-MM-dd}";
        }
    }
}