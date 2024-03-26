using Isango.Entities;
using Isango.Entities.Aot;
using Logger.Contract;
using ServiceAdapters.Aot.Aot.Commands.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System.IO.Compression;
using System.Text;
using Constant = ServiceAdapters.Aot.Constants.Constant;

namespace ServiceAdapters.Aot.Aot.Commands
{
    public class GetBulkPricingAvailabilityDetailsCommandHandler : CommandHandlerBase, IGetBulkPricingAvailabilityDetailsCommandHandler
    {
        private readonly ILogger _log;

        public GetBulkPricingAvailabilityDetailsCommandHandler(ILogger iLog) : base(iLog)
        {
            _log = iLog;
        }

        protected override object AotApiRequest<T>(T inputContext)
        {
            try
            {
                var optionAvailRequest = inputContext as OptionAvailRequest;
                if (optionAvailRequest == null)
                {
                    _log.Write(SerializeXml(inputContext), "AOT_API_Request", "AOT_API_Request", "AOT", "AOT");
                    return null;
                }

                optionAvailRequest.AgentId = AgentId;
                optionAvailRequest.Password = Password;

                HttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

                var content = new StringContent(SerializeXml(optionAvailRequest), Encoding.UTF8, Constant.ApplicationMediaType);
                var result = HttpClient.PostAsync(string.Empty, content);
                result.Wait();

                //Read the content of the result response from the server
                using (var stream = result.Result.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                using (var decompressed = new GZipStream(stream, CompressionMode.Decompress))
                using (var reader = new StreamReader(decompressed))
                {
                    try
                    {
                        var resultContent = reader.ReadToEnd();
                        return resultContent;
                    }
                    catch (Exception ex)
                    {
                        var teststring = result.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        _log.Write(SerializeXml(inputContext), teststring, "AotApiRequest", "AOT_API_STRING", "AOT");
                        _log.Write(SerializeXml(inputContext), HttpClient.DefaultRequestHeaders.AcceptEncoding.Count.ToString(), "AotApiRequest", "AOT_API_STRING", "AOT");
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "GetBulkPricingAvailabilityDetailsCommandHandler",
                            MethodName = "AotApiRequest",
                            Token = "AOT_CATCH_ERROR"
                        };
                        _log.Error(isangoErrorEntity, ex);
                        return teststring;
                    }
                    finally
                    {
                        _log.Write(SerializeXml(inputContext), "Disposing the reader", "AotApiRequest", "AOT_API_DISPOSE", "AOT");
                        reader.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Write(SerializeXml(inputContext), ex.InnerException?.ToString(), "AotApiRequest", "AOT_API_ERROR", "AOT");
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GetBulkPricingAvailabilityDetailsCommandHandler",
                    MethodName = "AotApiRequest",
                    Token = "AOT_ERROR"
                };
                _log.Error(isangoErrorEntity, ex);
                return null;
            }
        }

        protected override async Task<object> AotApiRequestAsync<T>(T inputContext)
        {
            var optionAvailRequest = inputContext as OptionAvailRequest;
            if (optionAvailRequest == null) return null;

            optionAvailRequest.AgentId = AgentId;
            optionAvailRequest.Password = Password;

            HttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            var content = new StringContent(SerializeXml(optionAvailRequest), Encoding.UTF8, Constant.ApplicationMediaType);
            var result = await HttpClient.PostAsync(string.Empty, content);

            //Read the content of the result response from the server
            using (var stream = await result.Content.ReadAsStreamAsync())
            using (Stream decompressed = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new StreamReader(decompressed))
            {
                var resultContent = reader.ReadToEnd();
                return resultContent;
            }
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var input = inputContext as AotCriteria;
            var opts = new Opts
            {
                Opt = input?.OptCode
            };
            var request = new OptionAvailRequest()
            {
                Opts = opts,
                DateFrom = input?.CheckinDate.ToString(Constant.DateFormatmmddyyhipen),
                DateTo = input?.CheckoutDate.ToString(Constant.DateFormatmmddyyhipen),
                IncludeRates = "1"
            };
            return request;
        }
    }
}