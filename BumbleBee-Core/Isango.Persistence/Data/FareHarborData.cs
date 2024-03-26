using Isango.Entities;
using System;
using System.Data;
using Util;
using Product = Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor.Product;

namespace Isango.Persistence.Data
{
    public class FareHarborData
    {
        /// <summary>
        /// Load FareHarbor product data
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Product LoadProductsData(IDataReader reader)
        {
            var product = new Product
            {
                FactsheetId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "TourId"),
                SupplierName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "shortname"),
                UserKey = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "userkey")
            };
            var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceID");
            product.ServiceId = !string.IsNullOrEmpty(serviceId) ? DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceID") : 0;
            product.CheckinDate = DateTime.Now;
            product.CheckoutDate = DateTime.Now.AddDays(Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constants.Constants.Days2FetchForFHBData)));
            return product;
        }

        /// <summary>
        /// Get FareHarbor user keys
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public FareHarborUserKey GetUserKeysData(IDataReader reader)
        {
            var key = new FareHarborUserKey
            {
                Id = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "id"),
                Currency = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "currency"),
                UserKey = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "userkey")
            };

            return key;
        }
    }
}