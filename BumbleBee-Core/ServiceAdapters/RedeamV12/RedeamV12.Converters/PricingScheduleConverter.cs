using Isango.Entities.RedeamV12;
using Newtonsoft.Json.Linq;
using ServiceAdapters.RedeamV12.RedeamV12.Converters.Contracts;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.GetProducts;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.PricingSchedule;
using System.Collections.Generic;
using System.Text.Json;
using Util;

namespace ServiceAdapters.RedeamV12.RedeamV12.Converters
{
    public class PricingScheduleConverter : ConverterBase, IPricingScheduleConverter
    {
        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override object Convert<T>(T response, T request)
        {

           
            Dictionary<string, Dictionary<string, List<PricingScheduleResponse>>> result = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<PricingScheduleResponse>>>>((response.ToString()));
            if (result == null) return null;
            return ConvertPricingScheduleData(result, request as string);
        }

        #region Private Methods

        private Dictionary<string, Dictionary<string, List<PricingScheduleResponse>>> ConvertPricingScheduleData(Dictionary<string, Dictionary<string, List<PricingScheduleResponse>>> pricingSchedule, string supplierId)
        {
            return pricingSchedule;
        }

        #endregion Private Methods
    }
 
}