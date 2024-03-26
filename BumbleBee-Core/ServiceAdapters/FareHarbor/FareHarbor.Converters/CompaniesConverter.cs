using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.FareHarbor.FareHarbor.Converters.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels;
using Util;

namespace ServiceAdapters.FareHarbor.FareHarbor.Converters
{
    public class CompaniesConverter : ConverterBase, ICompaniesConverter
    {
        public CompaniesConverter(ILogger logger) : base(logger)
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
            var result = SerializeDeSerializeHelper.DeSerialize<CompanyResponse>(objectResult as string);

            if (result != null)
            {
                var companiesList = ConvertCompaniesResult(result);
                return companiesList;
            }

            return null;
        }

        /// <summary>
        /// This method maps the API response to isango Contracts objects.
        /// </summary>
        /// <param name="companiesList">Input CompanyResponse model object</param>
        /// <returns>Isango.Contracts.Entities.Supplier Object</returns>
        private object ConvertCompaniesResult(CompanyResponse companiesList)
        {
            var supplierList = new List<Supplier>();
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            if (companiesList.Companies != null)
            {
                Parallel.ForEach(companiesList.Companies, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, company =>
                {
                    var supplier = new Supplier
                    {
                        Name = company.Name,
                        ShortName = company.ShortName,
                        Currency = company.Currency
                    };
                    supplierList.Add(supplier);
                });
            }

            return supplierList;
        }
    }
}