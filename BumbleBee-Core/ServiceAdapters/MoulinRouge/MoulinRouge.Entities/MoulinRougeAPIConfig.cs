using ServiceAdapters.MoulinRouge.Constants;
using System;
using System.Collections.Generic;
using Util;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities
{
    public class MoulinRougeAPIConfig
    {
        /// <summary>
        /// Provide instance that reads config values for MoulinRougeAPI
        /// </summary>
        public static MoulinRougeAPIConfig Instance { get; set; }

        public List<ValidRateIdContingentId> ValidRateIdContingentIDs { get; set; }

        /// <summary>
        /// Provided by Moulin Rouge API settings
        /// For moulin rouge Isango is a client and this represent its id that is required while we hit Order Confirm Call
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Provided by Moulin Rouge API settings
        /// AddressID that is required while we hit Order Confirm Call
        /// </summary>
        public string AddressId { get; set; }

        public int PaymentModeId { get; set; }

        private MoulinRougeAPIConfig()
        {
            ValidRateIdContingentIDs = GetValidRateIdContingentIds();
            ClientId = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ClientId));
            AddressId = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AddressId));
            PaymentModeId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.PaymentModeId));
        }

        public static MoulinRougeAPIConfig GetInstance()
        {
            return Instance ?? (Instance = new MoulinRougeAPIConfig());
        }

        /// <summary>
        /// Reads RateId:ContingentId from config file
        /// Except these all other combination are to be ignored while
        ///You might receive some availabilities for a Dinner Contigent with a Show with drinks id_Rate but you should not consider this as we do not sale it(it’s a bug that we can not solve at this time)
        ////For example you will have stocks with the contingent 82646 and rate 81622 but you should never use it.
        ////You should always use stocks with the association Id_rate/id_contingent given in the list.
        /// </summary>
        /// <returns>List of ValidRateIDContingentID for which price and availability can be fetched from Mouling Rouge Api, Ignore all other combinations</returns>
        private List<ValidRateIdContingentId> GetValidRateIdContingentIds()
        {
            var validRateIdContingentIds = new List<ValidRateIdContingentId>
            {
                GetValidRateIdContingentId(Constant.ShowWithDrinks, MoulinRougeServiceType.Show),
                GetValidRateIdContingentId(Constant.MistinguettMenu, MoulinRougeServiceType.Dinner),
                GetValidRateIdContingentId(Constant.VegetarianMenu, MoulinRougeServiceType.Dinner),
                GetValidRateIdContingentId(Constant.VeganMenu, MoulinRougeServiceType.Dinner),
                GetValidRateIdContingentId(Constant.ToulouseLautrecMenu, MoulinRougeServiceType.Dinner),
                GetValidRateIdContingentId(Constant.BelleEpoqueMenu, MoulinRougeServiceType.Dinner),
                GetValidRateIdContingentId(Constant.ChristmasDinner, MoulinRougeServiceType.Dinner),
                GetValidRateIdContingentId(Constant.ValentineDinner, MoulinRougeServiceType.Dinner)
            };
            return validRateIdContingentIds;
        }

        private ValidRateIdContingentId GetValidRateIdContingentId(string appConfigKey, MoulinRougeServiceType serviceType)
        {
            var validRateIdContingentId = new ValidRateIdContingentId();
            var keyItemFromConfig = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(appConfigKey)).Split(':');
            validRateIdContingentId.Name = appConfigKey;
            validRateIdContingentId.Type = serviceType;
            validRateIdContingentId.RateId = keyItemFromConfig.Length > 0 ? Convert.ToInt32(keyItemFromConfig[0]) : 0;
            validRateIdContingentId.ContingentId = keyItemFromConfig.Length > 0 ? Convert.ToInt32(keyItemFromConfig[1]) : 0;
            return validRateIdContingentId;
        }
    }

    /// <summary>
    /// For validation of RateID and ContingentID
    /// </summary>
    public class ValidRateIdContingentId
    {
        public string Name { get; set; }
        public MoulinRougeServiceType Type { get; set; }
        public int RateId { get; set; }
        public int ContingentId { get; set; }

        public ValidRateIdContingentId()
        {
            Name = string.Empty;
            Type = MoulinRougeServiceType.Undefined;
        }
    }
}