using Logger.Contract;
using ServiceAdapters.Aot.Aot.Commands.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System.IO.Compression;
using System.Text;
using Constant = ServiceAdapters.Aot.Constants.Constant;

namespace ServiceAdapters.Aot.Aot.Commands
{
    public class GetLocationCommandHandler : CommandHandlerBase, IGetLocationCommandHandler
    {
        public GetLocationCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object AotApiRequest<T>(T inputContext)
        {
            var getLocationsRequest = inputContext as GetLocationsRequest;
            if (getLocationsRequest == null) return null;

            getLocationsRequest.AgentId = AgentId;
            getLocationsRequest.Password = Password;

            HttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            var content = new StringContent(SerializeXml(getLocationsRequest), Encoding.UTF8,
                Constant.ApplicationMediaType);
            var result = HttpClient.PostAsync(string.Empty, content);
            result.Wait();

            //Read the content of the result response from the server
            using (var stream = result.Result.Content.ReadAsStreamAsync().Result)
            using (Stream decompressed = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new StreamReader(decompressed))
            {
                var resultContent = reader.ReadToEnd();
                return resultContent;
            }
        }

        protected override async Task<object> AotApiRequestAsync<T>(T inputContext)
        {
            var getLocationsRequest = inputContext as GetLocationsRequest;
            if (getLocationsRequest == null) return null;
            getLocationsRequest.AgentId = AgentId;
            getLocationsRequest.Password = Password;

            HttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            var content = new StringContent(SerializeXml(getLocationsRequest), Encoding.UTF8, Constant.ApplicationMediaType);
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
    }
}