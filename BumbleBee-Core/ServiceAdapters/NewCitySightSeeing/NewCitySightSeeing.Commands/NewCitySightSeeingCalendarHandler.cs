using Isango.Entities.NewCitySightSeeing;
using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing.Constants;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands.Contract;
using Util;
using Available = ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Availability;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands
{
    public class NewCitySightSeeingCalendarHandler : CommandHandlerBase, INewCitySightSeeingCalendarHandler
    {
        public NewCitySightSeeingCalendarHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var productAvailable = criteria as NewCitySightSeeingCriteria;
            var productCode = productAvailable.SupplierOptionNewCitySeeing;
            var toDate = productAvailable.CheckinDate.AddDays(productAvailable.Days2Fetch).ToString();

            //if (!string.IsNullOrEmpty(productAvailable?.ModalityCode))
            //{
            //    return
            //        (productAvailable != null)
            //            ? new Dictionary<string, string>()
            //           {
            //       {"ProductCode",productCode},
            //       {"VariantCode",productAvailable?.ModalityCode},
            //       {"ToDate", toDate},
            //       {"FromDate",productAvailable?.CheckinDate.ToString()},
            //           }
            //            : null;
            //}
            //else
            //{
                return
                                   (productAvailable != null)
                                       ? new Dictionary<string, string>()
                                      {
                   {"ProductCode",productCode},
                   {"ToDate", toDate},
                   {"FromDate",productAvailable?.CheckinDate.ToString()},
                                      }
                                       : null;
            //}
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(Available.AvailabilityResponse);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<Available.AvailabilityResponse>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override object GetResultsAsync(object input)
        {
            var url = $"{_newCitySightSeeingServiceURL}{Constant.ProductAvailability}";
            var queryParams = input as IDictionary<string, string>;

            var client = new AsyncClient
            {
                ServiceURL = url
            };
            return client.ConsumeGetService(GetHttpRequestHeaders(), queryParams);
        }
    }
}