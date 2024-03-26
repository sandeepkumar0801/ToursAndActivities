using Isango.Entities.RedeamV12;

using ServiceAdapters.RedeamV12.RedeamV12.Converters.Contracts;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.GetRates;

using System;
using System.Collections.Generic;

using Util;

namespace ServiceAdapters.RedeamV12.RedeamV12.Converters
{
    public class GetRatesConverter : ConverterBase, IGetRatesConverter
    {
        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public override object Convert<T>(T response)
        {
            return ConvertRatesResult(response as GetRatesResponse);
        }

        #region Private Methods

        private RatesWrapper ConvertRatesResult(GetRatesResponse ratesResponse)
        {
            if (ratesResponse?.Rates == null) return null;
            var rates = ratesResponse.Rates;

            var ratesDataList = new List<RateData>();
            var priceDataList = new List<PriceData>();
            var passengerTypeDataList = new List<PassengerTypeData>();

            foreach (var rate in rates)
            {
                var rateData = new RateData
                {
                    RateId = rate.Id.ToString(),
                    RateCode = rate.Code,
                    RateName = rate.Name,
                    Cancelable = rate.Cancelable,
                    Cutoff = rate.Cutoff,
                    Holdable = rate.Holdable,
                    HoldablePeriod = rate.HoldablePeriod,
                    Hours = SerializeDeSerializeHelper.Serialize(rate.Hours),
                    MaxTravelers = rate.MaxTravelers,
                    MinTravelers = rate.MinTravelers,
                    PartnerId = rate.PartnerId,
                    Type = rate.Type,
                    IsRefundable = rate.Refundable,
                    PricingType=rate.PricingType,
                    ProductId = rate.ProductId.ToString()
                };
                ratesDataList.Add(rateData);

                if(rate.Prices != null)
                {
                    var result = MapPriceData(rate.Prices, rate);
                    priceDataList.AddRange(result.Item1);
                    passengerTypeDataList.AddRange(result.Item2);
                }
            }

            var ratesWrapper = new RatesWrapper
            {
                Rates = ratesDataList,
                Prices = priceDataList,
                TravelerTypes = passengerTypeDataList
            };
            return ratesWrapper;
        }

        private Tuple<List<PriceData>, List<PassengerTypeData>> MapPriceData(List<Price> prices, Rate rate)
        {
            var priceDataList = new List<PriceData>();
            var passengerTypeDataList = new List<PassengerTypeData>();

            foreach (var price in prices)
            {
                var priceData = new PriceData
                {
                    ProductId = rate.ProductId.ToString(),
                    RateId = rate.Id.ToString(),
                    PriceId = price.Id.ToString(),
                    PriceName = price.Name,
                    Title = rate.Title,
                    Type = rate.Type,
                    Refundable = rate.Refundable,
                    Status = price.Status,
                    NetAmount = price.Net.Amount,
                    NetCurrency = price.Net.Currency,
                    RetailAmount = price.Retail.Amount,
                    RetailCurrency = price.Retail.Currency,
                    Labels = SerializeDeSerializeHelper.Serialize(price.Labels),
                    Version = rate.Version
                };
                priceDataList.Add(priceData);

                var travelerType = price.TravelerType;
                var passengerTypeData = new PassengerTypeData
                {
                    ProductId = rate.ProductId.ToString(),
                    RateId = rate.Id.ToString(),
                    PriceId = price.Id.ToString(),
                    AgeBand = travelerType.AgeBand,
                    MaxAge = travelerType.MaxAge,
                    MinAge = travelerType.MinAge,
                    Name = travelerType.Name
                };
                passengerTypeDataList.Add(passengerTypeData);
            }
            return new Tuple<List<PriceData>, List<PassengerTypeData>>(priceDataList, passengerTypeDataList);
        }

        #endregion Private Methods
    }
}