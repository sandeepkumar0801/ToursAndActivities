using Isango.Entities.Redeam;

using ServiceAdapters.Redeam.Redeam.Converters.Contracts;
using ServiceAdapters.Redeam.Redeam.Entities.GetSuppliers;

using System.Collections.Generic;

using Util;

namespace ServiceAdapters.Redeam.Redeam.Converters
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
                supplierDataList.Add(supplierData);
            }

            return supplierDataList;
        }

        #endregion Private Methods
    }
}