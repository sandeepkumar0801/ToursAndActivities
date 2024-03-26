using Isango.Entities;
using Isango.Entities.Redeam;

using ServiceAdapters.Redeam.Redeam.Converters.Contracts;
using ServiceAdapters.Redeam.Redeam.Entities.CreateHold;

using Util;

namespace ServiceAdapters.Redeam.Redeam.Converters
{
    public class CreateHoldConverter : ConverterBase, ICreateHoldConverter
    {
        /// <summary>
        /// This method used to convert API response to iSango Contracts objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override object Convert<T>(T response, T request)
        {
            var result = SerializeDeSerializeHelper.DeSerialize<CreateHoldResponse>(response.ToString());
            if (result == null) return null;

            return ConvertHoldResponse(result, request as SelectedProduct);
        }

        #region Private Methods

        private SelectedProduct ConvertHoldResponse(CreateHoldResponse createHoldResponse, SelectedProduct selectedProduct)
        {
            var redeamSelectedProduct = (RedeamSelectedProduct)selectedProduct;
            redeamSelectedProduct.HoldId = createHoldResponse.Hold.Id.ToString();
            redeamSelectedProduct.HoldStatus = createHoldResponse.Hold.Status;
            return selectedProduct;
        }

        #endregion Private Methods
    }
}