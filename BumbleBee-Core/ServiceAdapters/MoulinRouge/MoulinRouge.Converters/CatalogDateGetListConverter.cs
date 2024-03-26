using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.MoulinRouge.Converters.Contracts;
using System.Collections.Generic;
using CatalogDateGetDetailMulti = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetDetailMulti;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Converters
{
    public class CatalogDateGetListConverter : ConverterBase, ICatalogDateGetListConverter
    {
        public CatalogDateGetListConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// Retun Converted object by converting Response from API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        public override object Convert<T>(T inputContext)
        {
            var result = DeSerializeXml<CatalogDateGetDetailMulti.Response>(inputContext.ToString());
            var basketProductDetails = ConvertBasketResult(result);
            return basketProductDetails;
        }

        private List<SelectedProduct> ConvertBasketResult(CatalogDateGetDetailMulti.Response response)
        {
            var selectedProductList = new List<SelectedProduct>();
            /*
            foreach (var Reservation in basket.Reservations.Reservation)
            {
                var SelectedProduct = new SelectedProduct();
                SelectedProduct.ID = System.Convert.ToInt32(Reservation.Product.Id);
                SelectedProduct.Name = Reservation.Product.Text;
                SelectedProduct.ReservationId = System.Convert.ToInt32(Reservation.Id);
                SelectedProduct.TransactionPassword = basket.Transaction.Password;
                SelectedProduct.TransactionReference = System.Convert.ToInt32(basket.Transaction.Reference);
                SelectedProduct.ReservationExpiry = basket.Reservations.Expiry;
                SelectedProduct.Code = "Seats(" + Reservation.Block.Seats.Text + ")";

                SelectedProductList.Add(SelectedProduct);
            }
            */
            return selectedProductList;
        }
    }
}