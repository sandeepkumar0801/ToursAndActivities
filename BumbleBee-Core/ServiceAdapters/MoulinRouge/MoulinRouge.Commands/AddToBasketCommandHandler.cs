using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AllocSeatsAutomatic = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.AllocSeatsAutomatic;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands
{
    public class AddToBasketCommandHandler : CommandHandlerBase, IAddToBasketCommandHandler
    {
        public AddToBasketCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Return request required to hit API
        /// </summary>
        /// <param name="inputContext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var input = inputContext as MoulinRougeSelectedProduct;
            var requestObject = new AllocSeatsAutomatic.Request();

            if (input == null) return requestObject;

            var rq = requestObject.Body.ACP_AllocSeatsAutomaticRequest;
            rq.id_Catalog = 1;
            rq.id_CatalogDate = input.CatalogDateId;
            rq.listAllocRequest.ACPO_AllocSARequest.ListID_Category.Int = input.CategoryId;
            rq.listAllocRequest.ACPO_AllocSARequest.ListID_Contingent.Int = input.ContingentId;
            rq.listAllocRequest.ACPO_AllocSARequest.ListID_Bloc.Int = input.BlocId;
            rq.listAllocRequest.ACPO_AllocSARequest.ListID_Floor.Int = input.FloorId;
            rq.listAllocRequest.ACPO_AllocSARequest.ListQuantity.ACPO_AllocSARequestQuantity.ID_Rate = input.RateId;
            rq.listAllocRequest.ACPO_AllocSARequest.ListQuantity.ACPO_AllocSARequestQuantity.Quantity = input.Quantity;
            return requestObject;
        }

        /// <summary>
        /// Returns API Result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object GetResult<T>(T inputContext)
        {
            var requestObject = inputContext as AllocSeatsAutomatic.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = result.Content.ReadAsStringAsync();
            return responseText;
        }

        protected override async Task<object> GetResultAsync<T>(T inputContext)
        {
            var requestObject = inputContext as AllocSeatsAutomatic.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = await result.Content.ReadAsStringAsync();
            return responseText;
        }
    }
}