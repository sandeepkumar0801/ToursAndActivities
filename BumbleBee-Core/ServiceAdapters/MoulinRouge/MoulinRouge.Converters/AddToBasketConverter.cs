using Isango.Entities;
using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.MoulinRouge.Converters.Contracts;
using System;
using System.Linq;
using AllocSeatsAutomatic = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.AllocSeatsAutomatic;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Converters
{
    public class AddToCartConverter : ConverterBase, IAddToBasketConverter
    {
        public AddToCartConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectResult">string response Add To Basket Call</param>
        /// <returns></returns>
        public override object Convert<T>(T objectResult)
        {
            var response = objectResult as AllocSeatsAutomatic.Response;
            var convertedResult = ConvertBasketResult(response);
            return convertedResult;
        }

        private MoulinRougeSelectedProduct ConvertBasketResult(AllocSeatsAutomatic.Response response)
        {
            try
            {
                var resp = response.Body.ACP_AllocSeatsAutomaticRequestResponse;
                var selectedProduct = new MoulinRougeSelectedProduct();
                if (!resp.ACP_AllocSeatsAutomaticRequestResult || resp.result != 0) return null;

                var totalPrice = resp.listAllocResponse.ACPO_AllocSAResponse.ListAllocSeat
                    .Where(item => item.Seat_Detail.ID_Seat > 0 && item.Seat_Detail.ID_PhysicalSeat > 0)
                    .Select(item => item.AmountDetail[8]).FirstOrDefault();
                var seatIds = resp.listAllocResponse.ACPO_AllocSAResponse.ListAllocSeat.Where(item => item.Seat_Detail.ID_Seat > 0 && item.Seat_Detail.ID_PhysicalSeat > 0).Select(item => item.Seat_Detail.ID_Seat).Distinct().ToList();
                selectedProduct.Amount = totalPrice;
                selectedProduct.Ids = seatIds;
                selectedProduct.Price = totalPrice;
                selectedProduct.TemporaryOrderRowId = resp.listAllocResponse.ACPO_AllocSAResponse.ID_TemporaryOrderRow;
                selectedProduct.TemporaryOrderId = resp.id_TemporaryOrder;
                return selectedProduct;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MoulinRouge.AddToCartConverter",
                    MethodName = "ConvertBasketResult"
                };
                _logger.Error(isangoErrorEntity, ex);
                throw; //use throw as existing flow should not break bcoz of logging implementation.
            }
        }
    }
}