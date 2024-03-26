using Isango.Entities;
using Isango.Entities.RedeamV12;
using Logger.Contract;
using ServiceAdapters.RedeamV12.Constants;
using ServiceAdapters.RedeamV12.RedeamV12.Commands.Contracts;

using System;
using System.Threading.Tasks;

namespace ServiceAdapters.RedeamV12.RedeamV12.Commands
{
    public class PricingScheduleCommandHandler : CommandHandlerBase, IPricingScheduleCommandHandler
    {
        public PricingScheduleCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to fetch rates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object RedeamApiRequest<T>(T inputContext)
        {
            var input = inputContext as CanocalizationCriteria;
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = HttpClient.GetAsync(methodPath);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call to fetch rates asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> RedeamApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as CanocalizationCriteria;
            var methodPath = new Uri(GenerateMethodPath(input));

            var result = await HttpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private string GenerateMethodPath(CanocalizationCriteria criteria)
        {
            //https://booking.sandbox.redeam.io/v1.2/suppliers/fc49b925-6942-4df8-954b-ed7df10adf7e/
            //products /02f0c6cb-77ae-4fcc-8f4d-99bc0c3bee18/
            //pricing /schedule?start_date=2023-08-12&end_date=2023-08-12&
            //rate_id =0666f27f-2f16-4eba-91b7-28f08ce095d2

            return BaseAddress+ "/suppliers/"+ criteria.SupplierId+ 
                "/products/"+ criteria.ProductId+
                "/pricing/schedule?start_date=" + criteria.CheckinDate.ToString(Constant.DateTimeStringFormatSingle) +
                "&end_date="+ criteria.CheckoutDate.ToString(Constant.DateTimeStringFormatSingle) +
                "&rate_id="+ criteria.RateId;
        }

        #endregion Private Methods
    }
}