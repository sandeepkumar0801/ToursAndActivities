using Isango.Entities;
using Newtonsoft.Json.Linq;
using ServiceAdapters.SightSeeing.Constants;
using ServiceAdapters.SightSeeing.SightSeeing.Converters.Contract;
using ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels;
using Util;

namespace ServiceAdapters.SightSeeing.SightSeeing.Converters
{
    public class CancelTicketConverter : ConverterBase, ICancelTicketConverter
    {
        /// <summary>
        /// Convert cancel ticket result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override object Convert<T>(string response, T request)
        {
            var cancelResult = ConvertCancelResult(response, request as List<SelectedProduct>);
            return cancelResult;
        }

        /// <summary>
        /// Convert Cancellation Result
        /// </summary>
        /// <param name="response"></param>
        /// <param name="selectedProducts"></param>
        /// <returns></returns>
        public object ConvertCancelResult(object response, List<SelectedProduct> selectedProducts)
        {
            //Creating dictionary for availability reference id and it's api cancallation status
            var status = new Dictionary<string, bool>();
            selectedProducts?.ForEach(e => { status.Add(e.AvailabilityReferenceId, false); });
            if (response != null)
            {
                var jsonObj = JObject.Parse(response.ToString());
                var responseObject = SerializeDeSerializeHelper.DeSerialize<CancelResponseObj>(jsonObj.GetValue("obj").ToString());
                if (responseObject.Obj != null && responseObject.StatusCode == Constant.StatusCodeOK)
                    selectedProducts?.ForEach(e => { status[e.AvailabilityReferenceId] = true; });
            }
            return status;
        }
    }
}