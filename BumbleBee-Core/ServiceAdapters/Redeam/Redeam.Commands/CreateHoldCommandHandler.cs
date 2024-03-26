using Isango.Entities.Redeam;
using Logger.Contract;
using ServiceAdapters.Redeam.Constants;
using ServiceAdapters.Redeam.Redeam.Commands.Contracts;
using ServiceAdapters.Redeam.Redeam.Entities.CreateHold;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Util;

namespace ServiceAdapters.Redeam.Redeam.Commands
{
    public class CreateHoldCommandHandler : CommandHandlerBase, ICreateHoldCommandHandler
    {
        public CreateHoldCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call to acquire a hold
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object RedeamApiRequest<T>(T inputContext)
        {
            var input = inputContext as CreateHoldRequest;
            var methodPath = new Uri(GenerateMethodPath());

            var content = new StringContent(SerializeDeSerializeHelper.Serialize(input), Encoding.UTF8, Constant.ApplicationMediaType);

            var result = HttpClient.PostAsync(methodPath, content);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call to acquire a hold asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> RedeamApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as CreateHoldRequest;
            var methodPath = new Uri(GenerateMethodPath());

            var content = new StringContent(SerializeDeSerializeHelper.Serialize(input), Encoding.UTF8, Constant.ApplicationMediaType);

            var result = await HttpClient.PostAsync(methodPath, content);
            return ValidateApiResponse(result);
        }

        /// <summary>
        /// Create the supplier's input request from Isango entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var selectedProduct = inputContext as RedeamSelectedProduct;
            var createHoldRequest = new CreateHoldRequest
            {
                Hold = new Hold
                {
                    Items = PrepareItems(selectedProduct)
                }
            };
            return createHoldRequest;
        }

        #region Private Methods

        /// <summary>
        /// Create the API endpoint url
        /// </summary>
        /// <returns></returns>
        private string GenerateMethodPath()
        {
            return $"{BaseAddress}{UriConstants.CreateHold}";
        }

        private List<Item> PrepareItems(RedeamSelectedProduct redeamSelectedProduct)
        {
            var items = new List<Item>();
            var productOption = redeamSelectedProduct?.ProductOptions.FirstOrDefault(x => x.IsSelected);

            if (productOption == null) return items;
            var numberOfPassengers = productOption.TravelInfo.NoOfPassengers;

            foreach (var kvp in numberOfPassengers)
            {
                var item = new Item
                {
                    At = productOption.TravelInfo.StartDate,
                    PriceId = redeamSelectedProduct.PriceId.FirstOrDefault(x => x.Key == kvp.Key.ToString().ToUpperInvariant()).Value,
                    RateId = Guid.Parse(redeamSelectedProduct.RateId),
                    SupplierId = Guid.Parse(redeamSelectedProduct.SupplierId),
                    TravelerType = kvp.Key.ToString().ToUpperInvariant(),
                    Quantity = kvp.Value
                };
                items.Add(item);
            }
            return items;
        }

        #endregion Private Methods
    }
}