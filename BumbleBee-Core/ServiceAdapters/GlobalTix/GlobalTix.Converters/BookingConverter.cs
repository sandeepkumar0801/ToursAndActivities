using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Enums;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;

namespace ServiceAdapters.GlobalTix.GlobalTix.Converters.Contracts
{
    class BookingConverter : ConverterBase, IBookingConverter
    {
        public override object Convert(object objectResult)
        {
            throw new NotImplementedException();
        }

        public override object Convert(object objectResult, object input)
        {
            BookingRS bookRS = objectResult as BookingRS;
            BookInputContext bookInputContext = input as BookInputContext;
            if (bookRS == null || bookInputContext == null)
            {
                return null;
            }

            Booking bookRes = new Booking()
            {
                ReferenceNumber = bookRS.BookingData.BookReferenceNumber,
                Date = bookRS.BookingData.BookTime,
                Currency = new Currency() { IsoCode = bookRS.BookingData.CurrencyCode },
                SelectedProducts = new List<SelectedProduct>()
            };

            // Assumption is that same attraction will not be repeated in  
            // List<SelectedProduct> in BookInputContext
            foreach (SelectedProduct selProd in bookInputContext.SelectedProducts)
            {
                GlobalTixSelectedProduct gtSelProd = selProd as GlobalTixSelectedProduct;
                gtSelProd.APIDetails = new ApiExtraDetail();
                gtSelProd.APIDetails.SupplieReferenceNumber = bookRS.BookingData.BookReferenceNumber;
                gtSelProd.APIDetails.APITicketDetails = new List<ApiTicketDetail>();

				// Set QRCodeType as "LINK" and QRCode as eTicketURL irrespective of the ticketFormat type (BARCODE, QRCODE, PDFVOUCHER)
				gtSelProd.APIDetails.QRCodeType = Constant.QRCode_Type_Link;
				gtSelProd.APIDetails.QRCode = bookRS.BookingData.ETicketUrl;

                List<BookTicket> attrTickets = bookRS.BookingData.Tickets;//.FindAll(tkt => tkt.Attraction.Id == gtSelProd.ProductId);

                OptionBookingStatus bookStatus = OptionBookingStatus.Confirmed;
                foreach (BookTicket bookedTicket in attrTickets)
                {
                    ApiTicketDetail apiTktDtl = new ApiTicketDetail();
                    apiTktDtl.APIOrderId = bookedTicket.Id.ToString();
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

                bookRes.SelectedProducts.Add(gtSelProd);
            }

            return bookRes;
        }
    }
}
