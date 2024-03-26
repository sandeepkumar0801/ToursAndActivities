using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Isango.Entities.Booking.RequestModels
{
    public class GeneratePaymentLinkResponse
    {
          public string Value { get; set; }
          public string Currency { get; set; }
          public string CountryCode { get; set; }
          public string Description { get; set; }
          public string ExpiresAt { get; set; }
          public string Id { get; set; }
          public string MerchantAccount { get; set; }
          public string Reference { get; set; }
          public string ShopperLocale { get; set; }
          public string ShopperReference { get; set; }
          public string Url { get; set; }
          public string CustomerEmail { get; set; }
          public string CustomerLanguage { get; set; }
          public string TemporaryRefNo { get; set; }
    }
}