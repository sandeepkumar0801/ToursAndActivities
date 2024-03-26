using Isango.Entities.Redeam;

using ServiceAdapters.Redeam.Redeam.Converters.Contracts;
using ServiceAdapters.Redeam.Redeam.Entities.GetProducts;

using System.Collections.Generic;

using Util;

namespace ServiceAdapters.Redeam.Redeam.Converters
{
    public class GetProductsConverter : ConverterBase, IGetProductsConverter
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
            var result = SerializeDeSerializeHelper.DeSerialize<GetProductsResponse>(response.ToString());
            if (result == null) return null;

            return ConvertSupplierData(result, request as string);
        }

        #region Private Methods

        private List<ProductData> ConvertSupplierData(GetProductsResponse productsResponse, string supplierId)
        {
            if (productsResponse?.Products == null) return null;
            var products = productsResponse.Products;

            var productDataList = new List<ProductData>();
            foreach (var product in products)
            {
                if (product == null) continue;
                var productData = new ProductData
                {
                    ProductId = product.Id.ToString(),
                    SupplierId = supplierId,
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    PartnerId = product.PartnerId,
                    Version = product.Version,
                    Description = product.Description,
                    ProductHour = SerializeDeSerializeHelper.Serialize(product.Hours)
                };
                productDataList.Add(productData);
            }

            return productDataList;
        }

        #endregion Private Methods
    }
}