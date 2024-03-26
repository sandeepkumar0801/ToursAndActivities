using Isango.Entities;
using Isango.Entities.CitySightseeing;
using Logger.Contract;
using ServiceAdapters.SightSeeing.SightSeeing.Converters.Contract;
using ServiceAdapters.SightSeeing.SightSeeing.Entities;
using ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels;
using Util;

namespace ServiceAdapters.SightSeeing.SightSeeing.Converters
{
    public class IssuingConverter : ConverterBase, IIssuingConverter
    {
        protected readonly ILogger _logger;

        public IssuingConverter(ILogger logger)
        {
            _logger = logger;
        }

        public MethodType Converter { get; set; }

        public object Convert(object objectResult)
        {
            var result = (IssueResponse)objectResult;
            var issueResult = ConvertIssueResult(result);
            return issueResult;
        }

        private List<CitySightseeingSelectedProduct> ConvertIssueResult(IssueResponse response)
        {
            try
            {
                var lstSelectedProduct = new List<CitySightseeingSelectedProduct>();
                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                Parallel.ForEach(response.Response, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, (ticket) =>
                {
                    var selectedProduct = new CitySightseeingSelectedProduct
                    {
                        QrCode = ticket.QrCode,
                        Pnr = ticket.Pnr,
                        Id = System.Convert.ToInt32(ticket.TicketId.ToString().Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0])
                    };

                    lstSelectedProduct.Add(selectedProduct);
                });
                return lstSelectedProduct;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SightSeeing.IssuingConverter",
                    MethodName = "ConvertIssueResult"
                };
                _logger.Error(isangoErrorEntity, ex);
                throw; //use throw as existing flow should not break bcoz of logging implementation.
            }
        }
    }
}