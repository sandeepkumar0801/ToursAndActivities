using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Converters.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities.Products;
using Util;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Converters
{
    public class ProductsConverter : ConverterBase, IProductsConverter
    { 
    public ProductsConverter(ILogger logger) : base(logger)
    {
    }

        /// <summary>
        /// This method used to convert API response to isango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">string response Companies Call</param>
        /// <param name="criteria"></param>
        /// <returns>Isango.Contracts.Entities.Supplier List Object</returns>
        public override object Convert<T>(T objectResult, object criteria)
        {
            try
            {

                var result = SerializeDeSerializeHelper.DeSerialize<List<ProductResponse>>(objectResult as string);

                if (result != null)
                {
                    var companiesList = ConvertProductsResult(result);
                    return companiesList;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log the error, or take appropriate action
                Console.WriteLine($"An error occurred during conversion: {ex.Message}");
            }

            return null;
        }


        /// <summary>
        /// This method maps the API response to isango Contracts objects.
        /// </summary>
        /// <param name="companiesList">Input CompanyResponse model object</param>
        /// <returns>Isango.Contracts.Entities.Supplier Object</returns>
        private List<ExternalProducts> ConvertProductsResult(List<ProductResponse> products)
        {
            var externalProductsList = new List<ExternalProducts>();
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");

            foreach (var product in products)
            {
                if (product.options != null && product.options.Count > 0)
                {
                    Parallel.ForEach(product.options, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, option =>
                    {
                        var externalProduct = new ExternalProducts
                        {
                            CssProductId = product.id,
                            IsangoProductOptionId = option.externalId,
                            CssProductOptionId = option.id,
                            productName = product.name,
                            supplierId = product.supplierId
                        };

                        lock (externalProductsList) // Need to synchronize access to the list when adding items
                        {
                            externalProductsList.Add(externalProduct);
                        }
                    });
                }
            }

            return externalProductsList;
        }


    }
}
