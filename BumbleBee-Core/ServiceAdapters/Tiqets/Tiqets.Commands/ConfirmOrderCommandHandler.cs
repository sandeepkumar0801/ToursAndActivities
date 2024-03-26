using Isango.Entities.Tiqets;
using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;
using ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels;
using System.Text;
using Util;
using BookingRequest = Isango.Entities.Tiqets.BookingRequest;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
    public class ConfirmOrderCommandHandler : CommandHandlerBase, IConfirmOrderCommandHandler
    {
        public ConfirmOrderCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object TiqetsBookingApiRequest<T>(T inputContext)
        {
            var bookingRequest = inputContext as BookingRequest;
            var createOrderResponse = bookingRequest?.RequestObject as CreateOrderResponse;
            var confirmOrderRequest = new ConfirmOrderRequest { PaymentConfirmationToken = createOrderResponse?.PaymentConfirmationToken };
            var serializedRequest = SerializeDeSerializeHelper.SerializeWithContractResolver(confirmOrderRequest);
            var signedPayload = GetSignedPayload(serializedRequest, bookingRequest?.AffiliateId);
            var content = new StringContent(signedPayload, Encoding.UTF8, Constant.ApplicationOrJson);
            var url = FormUrl(createOrderResponse, bookingRequest?.TiquetsLanguageCode);
            using (var httpClient = AddRequestHeadersAndAddressToApi(bookingRequest?.AffiliateId))
            {
                var result = httpClient.PutAsync(url, content);
                result.Wait();
                return result.GetAwaiter().GetResult();
            }
        }

        //private string GetSignedPayload(string payload)
        //{
        //    var certificateFilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Constant.TiqetsCertificate}\\{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCertificateName)}";
        //    var password = string.Empty;
        //    var x509Certificate = new X509Certificate2(certificateFilePath, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
        //    var privateKey = x509Certificate.GetRSAPrivateKey();
        //    var signedPayload = JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);

        //    return signedPayload;
        //}

        private string FormUrl(CreateOrderResponse createOrderResponse, string languageCode)
        {
            languageCode = new[] { "ca", "cs", "da", "de", "el", "en", "es", "fr", "it", "ja", "ko", "nl", "pl", "pt", "ru", "sv", "zh" }.Contains(languageCode) ? languageCode : "en";
            return $"{UriConstant.ConfirmOrder}{createOrderResponse.OrderReferenceId}{UriConstant.BookingLanguage}{languageCode}";
        }
        
    }
}