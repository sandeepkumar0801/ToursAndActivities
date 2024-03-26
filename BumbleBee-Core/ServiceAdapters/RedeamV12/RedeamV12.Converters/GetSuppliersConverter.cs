using Isango.Entities.RedeamV12;

using ServiceAdapters.RedeamV12.RedeamV12.Converters.Contracts;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.GetSuppliers;

using System.Collections.Generic;
using System.Linq;
using Util;

namespace ServiceAdapters.RedeamV12.RedeamV12.Converters
{
    public class GetSuppliersConverter : ConverterBase, IGetSuppliersConverter
    {
        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public override object Convert<T>(T response)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<GetSuppliersResponse>(response.ToString());
            if (result == null) return null;

            return ConvertSupplierData(result);
        }

        #region Private Methods

        private List<SupplierData> ConvertSupplierData(GetSuppliersResponse suppliersResponse)
        {
            if (suppliersResponse?.Suppliers == null) return null;
            var suppliers = suppliersResponse.Suppliers;

            var supplierDataList = new List<SupplierData>();
            foreach (var supplier in suppliers)
            {
                if (supplier == null) continue;
                var supplierData = new SupplierData
                {
                    SupplierId = supplier.Id.ToString(),
                    SupplierName = supplier.Name,
                    SupplierCode = supplier.Code,
                    PartnerId = supplier.PartnerId,
                    Version = supplier.Version
                };

                if (supplier.Contacts != null && supplier.Contacts.Count > 0)
                {
                    supplierData.ContactEmail = supplier?.Contacts?.FirstOrDefault()?.Email;
                    supplierData.ContactName = supplier?.Contacts?.FirstOrDefault()?.Name;
                    supplierData.ContactPhone = supplier?.Contacts?.FirstOrDefault()?.Phone;
                    supplierData.ContactPrimary = supplier.Contacts.FirstOrDefault().Primary;
                    supplierData.ContactTitle = supplier?.Contacts?.FirstOrDefault()?.Title;
                }
                if (supplier.BusinessType != null && supplier.BusinessType.Count > 0)
                {
                    supplierData.BusinessType = string.Join(",", supplier?.BusinessType);
                }
                if (supplier.MainLocation != null)
                {
                    supplierData.Country = supplier?.MainLocation?.Country;
                    supplierData.City = supplier?.MainLocation?.City;
                    supplierData.Website = supplier?.MainLocation?.Website;
                }
                supplierDataList.Add(supplierData);
            }
            return supplierDataList;
        }

        #endregion Private Methods
    }
}