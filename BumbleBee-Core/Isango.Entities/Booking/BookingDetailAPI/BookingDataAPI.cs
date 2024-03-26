using System;
using System.Collections.Generic;
using System.Data;

namespace Isango.Entities.Booking.BookingDetailAPI
{
    public class BookingDataAPI : BookingDetailBase
    {
        public List<BookedProductDetailAPI> BookedProductDetailList { get; set; }

        public List<ChildDetailAPI> ChildDetailList { get; set; }

        public List<SupplierDataAPI> SupplierDataList { get; set; }

        public List<SupplierOrHotelAddressAPI> SupplierOrHotelAddressList { get; set; }

        public List<CustomerAPI> Customers { get; set; }

        public List<CancellationPrice> CancellationPricesList { get; set; }

        public BookingDataAPI(IDataReader result)
            : base(result)
        {
            VoucherType = "3";

            result.NextResult();
            while (result.Read())
            {
                var othersBookedProductDetail = new BookedProductDetailAPI(result);
                if (BookedProductDetailList == null)
                    BookedProductDetailList = new List<BookedProductDetailAPI>();
                BookedProductDetailList.Add(othersBookedProductDetail);
            }

            result.NextResult();
            while (result.Read())
            {
                var othersChildDetail = new ChildDetailAPI(result);
                if (ChildDetailList == null)
                    ChildDetailList = new List<ChildDetailAPI>();
                ChildDetailList.Add(othersChildDetail);
            }

            result.NextResult();
            while (result.Read())
            {
                var othersSupplierData = new SupplierDataAPI(result);
                if (SupplierDataList == null)
                    SupplierDataList = new List<SupplierDataAPI>();
                SupplierDataList.Add(othersSupplierData);
            }

            result.NextResult();
            while (result.Read())
            {
                var othersSupplierOrHotelAddress = new SupplierOrHotelAddressAPI(result);
                if (SupplierOrHotelAddressList == null)
                    SupplierOrHotelAddressList = new List<SupplierOrHotelAddressAPI>();
                SupplierOrHotelAddressList.Add(othersSupplierOrHotelAddress);
            }

            result.NextResult();
            while (result.Read())
            {
                var noteSubject = Convert.ToString(result["NOTESUBJECT"]).Trim();
                var noteText = Convert.ToString(result["NOTETEXT"]).Trim();

                var serviceId = noteSubject.Split('_')[0].Trim();
                var serviceOption = noteSubject.Split('_')[1].Trim();
                var type = noteSubject.Split('_')[2].Trim();

                BookedProductDetailAPI bookedProductDetail = null;
                if (BookedProductDetailList != null && BookedProductDetailList.Count > 0)
                {
                    bookedProductDetail = BookedProductDetailList.Find(x => (x.ServiceID == serviceId) && (x.ServiceOptionInServiceID == serviceOption));
                }
                if (bookedProductDetail != null)
                {
                    if (type == "Pickup Location")
                        bookedProductDetail.PickupLocation = noteText;
                    else if (type == "Special Request")
                        bookedProductDetail.SpecialRequest = noteText;
                    else if (type == "CONTRACTCOMMENT")
                        bookedProductDetail.ContractComment = noteText;
                }
            }
            result.NextResult();
            while (result.Read())
            {
                var customer = new CustomerAPI(result);
                if (Customers == null)
                    Customers = new List<CustomerAPI>();
                Customers.Add(customer);
            }
            result.NextResult();
            while (result.Read())
            {
                var cancellationPrice = new CancellationPrice();
                if (CancellationPricesList == null)
                    CancellationPricesList = new List<CancellationPrice>();

                if (!string.IsNullOrEmpty(Convert.ToString(result["CANCELLATIONAMOUNT"])))
                {
                    cancellationPrice.CancellationAmount = Convert.ToDecimal(result["CANCELLATIONAMOUNT"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(result["CANCELLATIONDATE"])))
                {
                    cancellationPrice.CancellationFromdate = Convert.ToDateTime(result["CANCELLATIONDATE"]);
                }
                CancellationPricesList.Add(cancellationPrice);
            }
        }
    }
}