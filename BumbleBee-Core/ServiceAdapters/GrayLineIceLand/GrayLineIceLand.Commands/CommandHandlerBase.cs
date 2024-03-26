using Logger.Contract;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands
{
    public abstract class CommandHandlerBase
    {
        private readonly ILogger _log;

        #region Constructor

        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _log = log;
        }

        #endregion Constructor

        public virtual async Task<object> ExecuteAsync(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            var res = await GetResultsAsync(input, inputContext.AuthToken);
            var response = (HttpResponseMessage)res;
            watch.Stop();
            if (response.IsSuccessStatusCode)
            {
                var hdr = new StringBuilder();
                hdr.Append("<!--\n");
                foreach (var header in response.Headers)
                {
                    hdr.Append(header.Key);
                    hdr.Append(" ~ ");
                    hdr.Append("\t\t");
                    hdr.Append(((string[])(header.Value))[0]);
                    hdr.Append("\n");
                }
                _log.WriteTimer(inputContext.MethodType.ToString(), token, "GrayLineIceLand", watch.Elapsed.ToString());
                _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, "GrayLineIceLand");

                return response;
            }

            return null;
        }

        public virtual async Task<object> ExecuteCancelAsync(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            var res = await GetResultsAsync(input, inputContext.AuthToken);
            watch.Stop();
            var response = SerializeDeSerializeHelper.DeSerialize<CancelBookingRS>(res.ToString());
            _log.WriteTimer(inputContext.MethodType.ToString(), token, "GrayLineIceLand", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, "GrayLineIceLand");
            return response;
        }

        public virtual object ExecuteCancel(InputContext inputContext, string token, out string requestXml, out string responseXml)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            var res = GetResults(input, inputContext.AuthToken, out requestXml, out responseXml);
            watch.Stop();
            var response = SerializeDeSerializeHelper.DeSerialize<CancelBookingRS>(res.ToString());
            _log.WriteTimer(inputContext.MethodType.ToString(), token, "GrayLineIceLand", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, "GrayLineIceLand");
            return response;
        }

        public virtual async Task<string> ExecuteStringAsync(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            var res = await GetStringResultsAsync(input);
            watch.Stop();
            _log.WriteTimer(inputContext.MethodType.ToString(), token, "GrayLineIceLand", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(input), res, inputContext.MethodType.ToString(), token, "GrayLineIceLand");
            return res;
        }

        protected abstract Task<object> GetResultsAsync(object input, string authString);

        protected abstract Task<string> GetStringResultsAsync(object input);

        #region Synchronous methods

        public virtual object Execute(InputContext inputContext, string token, out string requestXml, out string responseXml)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            var res = GetResults(input, inputContext.AuthToken, out requestXml, out responseXml);
            watch.Stop();
            var response = res.ToString();
            _log.WriteTimer(inputContext.MethodType.ToString(), token, "GrayLineIceLand", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(input), res.ToString(), inputContext.MethodType.ToString(), token, "GrayLineIceLand");
            return response;
        }

        public virtual object Execute(InputContext inputContext, string token)
        {
            var watch = Stopwatch.StartNew();
            var input = CreateInputRequest(inputContext);
            var res = GetResults(input, inputContext.AuthToken);
            watch.Stop();
            var response = res?.ToString();
            Task.Run(() => _log.WriteTimer(inputContext.MethodType.ToString(), token, "GrayLineIceLand", watch.Elapsed.ToString()));
            Task.Run(() => _log.Write(SerializeDeSerializeHelper.Serialize(input), response, inputContext.MethodType.ToString(), token, "GrayLineIceLand"));
            return response;
        }

        protected abstract object CreateInputRequest(InputContext inputContext);

        protected abstract object GetResults(object input, string authString);

        protected abstract object GetResults(object input, string authString, out string requestXml,
            out string responseXml);

        #endregion Synchronous methods
    }
}