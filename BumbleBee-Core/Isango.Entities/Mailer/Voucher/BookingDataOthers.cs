using Isango.Entities.Booking.BookingDetailAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Isango.Entities.Mailer.Voucher
{
    public class BookingDataOthers : BookingDetailBase
    {
        public List<OthersBookedProductDetail> BookedProductDetailList { get; set; }
        public List<OthersChildDetail> ChildDetailList { get; set; }
        public List<OthersSupplierData> SupplierDataList { get; set; }
        public List<OthersSupplierOrHotelAddress> SupplierOrHotelAddressList { get; set; }
        public List<Customer> Customers { get; set; }
        public List<CancellationPrice> CancellationPricesList { get; set; }
        public List<BookingAgeGroup> BookingAgeGroupList { get; set; }
        public List<BarCodeData> BarCodeList { get; set; }

        public List<MoulinRougePDF> MoulinRougePDFBytes { get; set; }

        public BookingDataOthers(IDataReader result)
            : base(result)
        {
            VoucherType = "3";

            result.NextResult();
            while (result.Read())
            {
                var othersBookedProductDetail = new OthersBookedProductDetail(result);
                if (BookedProductDetailList == null)
                    BookedProductDetailList = new List<OthersBookedProductDetail>();
                BookedProductDetailList.Add(othersBookedProductDetail);
            }

            result.NextResult();
            while (result.Read())
            {
                var othersChildDetail = new OthersChildDetail(result);
                if (ChildDetailList == null)
                    ChildDetailList = new List<OthersChildDetail>();
                ChildDetailList.Add(othersChildDetail);
            }

            result.NextResult();
            while (result.Read())
            {
                var othersSupplierData = new OthersSupplierData(result);
                if (SupplierDataList == null)
                    SupplierDataList = new List<OthersSupplierData>();
                SupplierDataList.Add(othersSupplierData);
            }

            result.NextResult();
            while (result.Read())
            {
                var othersSupplierOrHotelAddress = new OthersSupplierOrHotelAddress(result);
                if (SupplierOrHotelAddressList == null)
                    SupplierOrHotelAddressList = new List<OthersSupplierOrHotelAddress>();
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

                OthersBookedProductDetail bookedProductDetail = null;
                if (BookedProductDetailList != null && BookedProductDetailList.Count > 0)
                {
                    bookedProductDetail = BookedProductDetailList.Find(x => (x.ServiceId == serviceId) && (x.ServiceOptionInServiceId == serviceOption));
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
                var customer = new Customer(result);
                if (Customers == null)
                    Customers = new List<Customer>();
                Customers.Add(customer);
            }
            result.NextResult();
            while (result.Read())
            {
                var cancellationPrice = new CancellationPrice();
                if (CancellationPricesList == null)
                    CancellationPricesList = new List<CancellationPrice>();

                if (!String.IsNullOrEmpty(Convert.ToString(result["CANCELLATIONAMOUNT"])))
                {
                    cancellationPrice.CancellationAmount = Convert.ToDecimal(result["CANCELLATIONAMOUNT"]);
                }
                if (!String.IsNullOrEmpty(Convert.ToString(result["CANCELLATIONDATE"])))
                {
                    cancellationPrice.CancellationFromdate = Convert.ToDateTime(result["CANCELLATIONDATE"]);
                }
                if (!String.IsNullOrEmpty(Convert.ToString(result["bookedoptionid"])))
                {
                    cancellationPrice.BookedOptionID = Convert.ToInt32(result["bookedoptionid"]);
                }
                CancellationPricesList.Add(cancellationPrice);
            }
            result.NextResult();
            while (result.Read())
            {
                var bookingAgeGroup = new BookingAgeGroup();
                if (BookingAgeGroupList == null)
                {
                    BookingAgeGroupList = new List<BookingAgeGroup>();
                }
                if (!String.IsNullOrEmpty(Convert.ToString(result["BOOKEDOPTIONID"])))
                {
                    bookingAgeGroup.BookedOptionId = Convert.ToString(result["BOOKEDOPTIONID"]);
                }
                if (!String.IsNullOrEmpty(Convert.ToString(result["AgeGroupDesc"])))
                {
                    bookingAgeGroup.AgeGroupDesc = Convert.ToString(result["AgeGroupDesc"]);
                }
                if (!String.IsNullOrEmpty(Convert.ToString(result["BOOKEDPASSENGERRATESELLAMOUNT"])))
                {
                    bookingAgeGroup.PaxSellAmount = Convert.ToDecimal(result["BOOKEDPASSENGERRATESELLAMOUNT"]);
                }
                if (!String.IsNullOrEmpty(Convert.ToString(result["suppliercostamount"])))
                {
                    bookingAgeGroup.PaxSupplierCostAmount = Convert.ToDecimal(result["suppliercostamount"]);
                }
                if (!String.IsNullOrEmpty(Convert.ToString(result["PASSENGERTYPEID"])))
                {
                    try
                    {
                        bookingAgeGroup.PassengerTypeId = Convert.ToInt32(result["PASSENGERTYPEID"]);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    try
                    {
                        bookingAgeGroup.PassengerType = Convert.ToString(result["PASSENGERTYPE"]);
                    }
                    catch (Exception)
                    {
                    }
                }
                bookingAgeGroup.PaxCount = Convert.ToInt32(result["PaxCount"]);
                BookingAgeGroupList.Add(bookingAgeGroup);
            }
            result.NextResult();
            while (result.Read())
            {
                var barCodeData = new BarCodeData();
                if (BarCodeList == null)
                {
                    BarCodeList = new List<BarCodeData>();
                }

                barCodeData.BookedOptionId = GetStringValue(result, "BOOKEDOPTIONID");

                barCodeData.BarCode = GetStringValue(result, "BarCode");

                barCodeData.FiscalNumber = GetStringValue(result, "FiscalNumber");

                barCodeData.CodeType = GetStringValue(result, "CodeType");

                barCodeData.CodeValue = GetStringValue(result, "CodeValue");

                barCodeData.IsResourceApply = GetBoolValue(result, "IsResourceApply");

                barCodeData.ResourceType = GetStringValue(result, "ResourceType");

                barCodeData.ResouceRemoteUrl = GetStringValue(result, "ResouceRemoteUrl");

                barCodeData.ResourceLocal = GetStringValue(result, "ResourceLocal");

                barCodeData.PassengerType = GetStringValue(result, "PassengerType");

                barCodeData.PassengerCount = GetIntValue(result, "PassengerCount");

                BarCodeList.Add(barCodeData);
            }
            result.NextResult();
            while (result.Read())
            {
                var moulinRougePDF = new MoulinRougePDF();
                if (MoulinRougePDFBytes == null)
                {
                    MoulinRougePDFBytes = new List<MoulinRougePDF>();
                }
                moulinRougePDF.ApiOrderId = GetStringValue(result, "apiorderid");
                moulinRougePDF.ApiTicketGUWID = GetStringValue(result, "apiticketguwid");
                moulinRougePDF.BookingreRerenceNumber = GetStringValue(result, "bookingreferencenumber");
                moulinRougePDF.ServiceOptionId = GetIntValue(result, "serviceoptionid");
                moulinRougePDF.ApiVoucherByte = GetBytesValue(result, "apivoucherbyte");

                MoulinRougePDFBytes.Add(moulinRougePDF);
            }

            this.UsefulDowloads = new List<UsefulDownload>();
            result.NextResult();
            while (result.Read())
            {
                var usefulDownload = new UsefulDownload();
                if (MoulinRougePDFBytes == null)
                {
                    MoulinRougePDFBytes = new List<MoulinRougePDF>();
                }
                usefulDownload.BookedOptionId = GetIntValue(result, "BOOKEDOPTIONID");
                usefulDownload.DownloadText = GetStringValue(result, "Downloadtext");
                usefulDownload.DownloadLink = GetStringValue(result, "Downloadlink");
                usefulDownload.LinkOrder = GetIntValue(result, "linkOrder");

                UsefulDowloads.Add(usefulDownload);
            }
            if (UsefulDowloads?.Count > 1)
            {
                UsefulDowloads = UsefulDowloads?.OrderBy(x => x.BookedOptionId)?.ThenBy(y => y.LinkOrder)?.ToList();
            }
        }

        /// <summary>
        /// Get String value from db result
        /// </summary>
        /// <param name="result"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetStringValue(IDataReader result, string key)
        {
            var reader = result as IDataReader;
            var value = string.Empty;
            try
            {
                if (reader[key] != DBNull.Value)
                {
                    value = Convert.ToString(reader[key]);
                }
            }
            catch (Exception ex)
            {
            }
            return value;
        }

        public int GetIntValue(IDataReader result, string key)
        {
            var reader = result as IDataReader;
            var value = 0;
            try
            {
                if (reader[key] != DBNull.Value)
                {
                    value = Convert.ToInt32(reader[key]);
                }
            }
            catch (Exception ex)
            {
            }
            return value;
        }

        /// <summary>
        /// Get Boolean value from db
        /// </summary>
        /// <param name="result"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolValue(IDataReader result, string key)
        {
            var reader = result as IDataReader;
            bool value = false;
            try
            {
                if (reader[key] != DBNull.Value)
                {
                    value = Convert.ToBoolean(reader[key]);
                }
            }
            catch (Exception ex)
            {
            }
            return value;
        }

        public byte[] GetBytesValue(IDataReader result, string key)
        {
            var reader = result as IDataReader;
            byte[] value = null;
            try
            {
                if (reader[key] != DBNull.Value)
                {
                    value = (byte[])reader[key];
                }
            }
            catch (Exception ex)
            {
            }
            return value;
        }
    }
}