using Isango.Entities.Activities;
using Isango.Entities.MoulinRouge;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities.AllocSeatsAutomatic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceAdapters.MoulinRouge
{
    public interface IMoulinRougeAdapter
    {
        Task<object> AllocSeatsAutomaticAsync(MoulinRougeCriteria inputContext, string token);

        Task<object> GetTempOrderGetDetailAsync(MoulinRougeCriteria inputContextv, string token);

        Task<bool> ReleaseSeatsAsync(MoulinRougeCriteria inputContext, string token);

        Task<object> TempOrderGetSendingFeesAsync(string temporaryOrderId, string token);

        List<Activity> GetConvertedActivtyDateAndPrice(DateTime dateFrom, DateTime dateTo, int quantity, string token);

        MoulinRougeSelectedProduct AddToCart(MoulinRougeSelectedProduct inputContext, string token);

        MoulinRougeSelectedProduct OrderConfirmCombined(MoulinRougeSelectedProduct selectedProduct, out string requestXml, out string responseXml, string token);

        Task<byte[]> GetOrderEticketAsync(string orderId, string guid, string token);

        Response AddToCartAPI(MoulinRougeSelectedProduct inputContext, string token);
    }
}