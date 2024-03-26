using Isango.Entities.Tiqets;
using Jose;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Entities;
using ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Util;
using BookingRequest = Isango.Entities.Tiqets.BookingRequest;

namespace ServiceAdapters.Tiqets.Tiqets.Commands.Contracts
{
    public class CancelOrderCommandHandler : CommandHandlerBase, ICancelOrderCommandHandler
    {
        public CancelOrderCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object TiqetsBookingApiRequest<T>(T inputContext)
        {
            var bookingRequest = inputContext as BookingRequest;
            var tiqetsProduct = bookingRequest?.RequestObject as TiqetsSelectedProduct;
            var cancelOrderRequest = new CancelOrderRequest { CancellationReason = CancellationReasonEnum.OTHER.ToString() };
            var serializedRequest = SerializeDeSerializeHelper.SerializeWithContractResolver(cancelOrderRequest);
            var signedPayload = GetSignedPayload(serializedRequest);
            var content = new StringContent(signedPayload, Encoding.UTF8, Constant.ApplicationOrJson);
            var url = FormUrl(tiqetsProduct, bookingRequest?.LanguageCode);
            using (var httpClient = AddRequestHeadersAndAddressToApi(bookingRequest?.AffiliateId))
            {
                //TODO
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = content,
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetUri) + url)
                };
                var result = httpClient.SendAsync(request);
                result.Wait();
                return result.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var bookingRequest = inputContext as BookingRequest;
            return bookingRequest;
        }

        private string GetSignedPayload(string payload)
        {
            var certificateFilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Constant.TiqetsCertificate}\\{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCertificateName)}";
            var password = string.Empty;
            var x509Certificate = new X509Certificate2(certificateFilePath, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            var privateKey = x509Certificate.GetRSAPrivateKey();
            var signedPayload = JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);

            return signedPayload;
        }
        
        private string FormUrl(TiqetsSelectedProduct tiqetsProduct, string languageCode)
        {
            return $"{UriConstant.CancelOrder}{tiqetsProduct.OrderReferenceId}";
        }
    }
}
