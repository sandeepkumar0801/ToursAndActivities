using Isango.Entities.GoogleMaps;
using Logger.Contract;
using ServiceAdapters.GoogleMaps.Constants;
using ServiceAdapters.GoogleMaps.GoogleMaps.Commands.Contracts;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO;
using System;
using System.Collections.Generic;
using Util;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Commands
{
    internal class MerchantFeedCommandHandler : CommandHandlerBase, IMerchantFeedCommandHandler
    {
        #region Constructor

        public MerchantFeedCommandHandler(ILogger log) : base(log)
        {
        }

        #endregion Constructor

        #region Protected Method

        protected override object MapFeed<T>(T inputContext)
        {
            var merchantFeeds = inputContext as List<MerchantFeed>;
            var googleMerchantFeeds = new List<Merchant>();
            if (merchantFeeds != null)
            {
                foreach (var merchantFeed in merchantFeeds)
                {
                    var googleMerchantFeed = new Merchant
                    {
                        MerchantId = merchantFeed.Supplierid?.Trim(),
                        Category = merchantFeed.Category?.Trim(),
                        Geo = new Geo
                        {
                            UnstructuredAddress = $"{ merchantFeed.Addressline1?.Trim()} {merchantFeed.Addressline2?.Trim()} {merchantFeed.Addresscity?.Trim()} {merchantFeed.Addresspostcode?.Trim()} {merchantFeed.Countryname?.Trim()}"
                        },
                        Name = merchantFeed.Suppliername,
                        TaxRate = new TaxRate
                        {
                            MicroPercent = 0
                        },
                        Telephone = merchantFeed.Addresstelephonenumber,
                        TokenizationConfig = new TokenizationConfig
                        {
                            TokenizationParameters = new Dictionary<string, string>
                        {
                            { Constant.Gateway, ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:Gateway") },
                            { Constant.GatewayMerchantId, ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:GatewayMerchantId") }
                        }
                        }
                    };
                    googleMerchantFeeds.Add(googleMerchantFeed);
                }
            }
            var result = new Merchants
            {
                Merchant = googleMerchantFeeds,
                Metadata = new Metadata
                {
                    GenerationTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    TotalShards = 1,
                    ProcessingInstruction = Constant.ProcessingInstruction
                }
            };
            return SerializeDeSerializeHelper.Serialize(result);
        }

        #endregion
    }
}