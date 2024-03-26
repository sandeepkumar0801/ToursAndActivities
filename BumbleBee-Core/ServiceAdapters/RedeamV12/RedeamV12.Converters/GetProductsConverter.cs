using Isango.Entities.RedeamV12;

using ServiceAdapters.RedeamV12.RedeamV12.Converters.Contracts;
using ServiceAdapters.RedeamV12.RedeamV12.Entities.GetProducts;

using System.Collections.Generic;

using Util;

namespace ServiceAdapters.RedeamV12.RedeamV12.Converters
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
                    ProductId = System.Convert.ToString(product.Id),
                    SupplierId = product.SupplierId?? supplierId,
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    PartnerId = product?.Extensions?.PasshubvendorId,
                    Version = product.Version,
                    Description = product.Description,
                    ProductHour = SerializeDeSerializeHelper.Serialize(product.Hours),
                    Title= product.Title
                };
                if (product?.Location != null)
                {
                    productData.LocationName = product.Location?.Name;
                    productData.LocationNotes = product.Location?.Notes;
                    productData.LocationWebsite = product.Location?.Website;

                    if(product.Location.Address!=null)
                    {
                        productData.GooglePlaceId = product.Location.Address?.GooglePlaceId;
                        productData.Region = product.Location.Address?.Region;
                    }
                }
                productDataList.Add(productData);
            }

            return productDataList;
        }

        #endregion Private Methods
    }
}