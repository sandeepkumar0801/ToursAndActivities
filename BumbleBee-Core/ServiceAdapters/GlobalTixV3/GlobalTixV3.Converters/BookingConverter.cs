using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Canocalization;
using Isango.Entities.Enums;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters.Contracts
{
    class BookingConverter : ConverterBase, IBookingConverter
    {
        public override object Convert(object objectResult)
        {
            throw new NotImplementedException();
        }

        public override object Convert(object objectResult, object objectResultDetail, object input)
        {
    

            var bookRS = Util.SerializeDeSerializeHelper.DeSerialize<GlobalTix.Entities.BookingRS>(System.Convert.ToString(objectResult));
            var bookDetailRS = Util.SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.BookingDetailsResponse>(System.Convert.ToString(objectResultDetail));


            var selectedProducts = input as List<SelectedProduct>;
            var selectedProduct = selectedProducts.FirstOrDefault();
            if (objectResult == null || selectedProducts == null)
            {
                return null;
            }

           var gtSelProd = selectedProduct as CanocalizationSelectedProduct;
            gtSelProd.APIDetails = new ApiExtraDetail
            {
                SupplieReferenceNumber = bookRS.DataBooking.ReferenceNumber
            };

            gtSelProd.APIDetails.APITicketDetails = new List<ApiTicketDetail>();
            gtSelProd.APIDetails.QRCodeType = Constant.QRCode_Type_Link;
            gtSelProd.APIDetails.QRCode = bookDetailRS.dataBookingDetail.ETicketUrl;

            List<TicketBookingDetail> attrTickets = bookDetailRS.dataBookingDetail.TicketsBookingDetail;

            OptionBookingStatus bookStatus = OptionBookingStatus.Confirmed;

            foreach (TicketBookingDetail bookedTicket in attrTickets)
            {
                ApiTicketDetail apiTktDtl = new ApiTicketDetail();
                apiTktDtl.APIOrderId = bookedTicket.Id.ToString();
                //apiTktDtl.CodeValue = bookedTicket.QRCode;
                //apiTktDtl.QRCodeType = bookedTicket.TicketFormat;
                gtSelProd.APIDetails.APITicketDetails.Add(apiTktDtl);

                if (Constant.Book_Status_Valid.Equals(bookedTicket.Status.Name) == false)
                {
                    bookStatus = OptionBookingStatus.Failed;
                }
            }

            ProductOption actOpt = gtSelProd.ProductOptions.FirstOrDefault(x => x.IsSelected);
            if (actOpt != null)
            {
                actOpt.BookingStatus = bookStatus;
            }

            return gtSelProd;
        }
        public override object Convert(object objectResult, object input)
        {
            throw new NotImplementedException();
        }
    }
}
