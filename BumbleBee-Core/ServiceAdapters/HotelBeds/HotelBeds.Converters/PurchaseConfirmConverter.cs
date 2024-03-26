using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Entities.Region;
using Logger.Contract;
using ServiceAdapters.HotelBeds.Constants;
using ServiceAdapters.HotelBeds.HotelBeds.Converters.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceAdapters.HotelBeds.HotelBeds.Converters
{
    public class PurchaseConfirmConverter : ConverterBase, IPurchaseConfirmConverter
    {
        public PurchaseConfirmConverter(ILogger logger) : base(logger)
        {
        }

        public object Convert(object objectresult)
        {
            var purchaseConfirmData = (PurchaseConfirmRs)objectresult;
            if (objectresult != null)
            {
                return ConvertPurchaseResult(purchaseConfirmData);
            }

            return null;
        }

        private List<HotelBedsSelectedProduct> ConvertPurchaseResult(PurchaseConfirmRs purchaseData)
        {
            var purchaseItems = new List<HotelBedsSelectedProduct>();

            //var productOptions = new List<ProductOption>();
            foreach (var svc in purchaseData.Purchase.ServiceList)
            {
                try
                {
                    var selectedService = new HotelBedsSelectedProduct
                    {
                        EchoToken = purchaseData.EchoToken,
                        PurchaseToken = purchaseData.Purchase.PurchaseToken,
                        TimeToExpiration = DateTime.Now.AddMilliseconds(purchaseData.Purchase.TimeToExpiration),
                        CartStatus = purchaseData.Purchase.Status,
                        FileNumber = purchaseData.Purchase.FileNumber,
                        OfficeCode = purchaseData.Purchase.IncomingOfficeCode,
                        SPUI = svc.Spui,
                        ServiceStatus = svc.Status,
                        VatNumber = svc.Supplier.VatNumber,
                        SupplierName = svc.Supplier.Name,
                        Price = svc.TotalAmount,
                        SupplierCurrency = svc.Currency
                    };

                    if (selectedService.Regions == null)
                        selectedService.Regions = new List<Region>();

                    #region HotelInfo

                    selectedService.Name = svc.TicketInfo.Name;
                    selectedService.Code = svc.TicketInfo.Code;
                    var region = new Region
                    {
                        Name = svc.TicketInfo.Destination == null ? "" : svc.TicketInfo.Destination.DestinationName
                    };
                    selectedService.Regions.Add(region);

                    #endregion HotelInfo

                    // Travel Info
                    var travelInfo = new TravelInfo();
                    if (travelInfo.NoOfPassengers == null)
                    {
                        travelInfo.NoOfPassengers = new Dictionary<PassengerType, int>();
                    }

                    travelInfo.NoOfPassengers.Add(PassengerType.Adult, svc.PassengerDetails.AdultCount);
                    travelInfo.NoOfPassengers.Add(PassengerType.Child, svc.PassengerDetails.ChildCount);

                    if (svc.PassengerDetails.GuestList.CustomerList.FindAll(g => g.Type.Equals("CH")) != null)
                    {
                        var children = svc.PassengerDetails.GuestList.CustomerList.FindAll(g => g.Type.Equals("CH"));
                        travelInfo.Ages = new Dictionary<PassengerType, int>();
                        if (children.Count > 0)
                            travelInfo.Ages.Add(PassengerType.Child, children.FirstOrDefault()?.Age ?? 0);
                    }

                    travelInfo.StartDate = svc.DateFrom;
                    var difference = svc.DateTo - svc.DateFrom;
                    travelInfo.NumberOfNights = difference.Days;

                    //Add Customers
                    var customers = new List<Customer>();

                    customers.AddRange(svc.PassengerDetails.GuestList.CustomerList.Select(guest => new Customer
                    {
                        Age = guest.Age,
                        PassengerType = (guest.Type.Equals(Constant.Ad) ? PassengerType.Adult : PassengerType.Child),
                        FirstName = guest.Name,
                        LastName = guest.LastName
                    }));

                    var actOption = new ActivityOption
                    {
                        #region ActivityOption

                        TravelInfo = travelInfo,
                        Customers = customers,
                        Name = svc.AvailableModality.Name,
                        IsSelected = true
                    };

                    //Price and Currency
                    var actPrice = new Price
                    {
                        Currency = new Currency
                        {
                            Name = svc.Currency,
                            IsoCode = purchaseData.Purchase.Currency
                        },
                        Amount = svc.TotalAmount
                    };
                    actOption.BasePrice = actPrice;

                    selectedService.Price = svc.TotalAmount;

                    //Add Contract
                    selectedService.ServiceContract = new Isango.Entities.HotelBeds.Contract
                    {
                        Name = svc.Contract.Name,
                        ClassificationCode = svc.Contract.Classification
                    };
                    var actComments = new List<Isango.Entities.HotelBeds.Comment>();

                    actOption.Contract = new Isango.Entities.Contract
                    {
                        Name = svc.Contract.Name,
                        ClassificationCode = svc.Contract.Classification
                    };

                    foreach (var comm in svc.Contract.Comments)
                    {
                        var comment = new Isango.Entities.HotelBeds.Comment
                        {
                            CommentText = comm.CommentText,
                            Type = comm.Type
                        };
                        actComments.Add(comment);
                    }
                    selectedService.ServiceContract.Comments = actComments;
                    selectedService.ServiceContract.Classification = svc.Contract.Classification;
                    selectedService.ServiceContract.InComingOfficeCode = svc.Contract.IncomingOffice;

                    actOption.Contract.Comments = actComments;
                    actOption.Contract.Classification = svc.Contract.Classification;
                    actOption.Contract.InComingOfficeCode = svc.Contract.IncomingOffice;

                    if (svc.CancellationCharges != null && svc.CancellationCharges.Count > 0)
                    {
                        var cancellationCost = new List<CancellationPrice>();
                        foreach (var charge in svc.CancellationCharges)
                        {
                            var price = new CancellationPrice
                            {
                                CancellationToDate = svc.CancellationPolicy.ToDate,
                                CancellationFromdate = svc.CancellationPolicy.FromDate,
                                CancellationAmount = svc.CancellationPolicy.Amount
                            };
                            cancellationCost.Add(price);
                        }
                        actOption.CancellationPrices = cancellationCost;
                    }

                    #endregion ActivityOption

                    //productOptions.Add(actOption);
                    selectedService.ProductOptions = new List<ProductOption>
                {
                    actOption
                };

                    if (svc.ServiceDetails != null && svc.ServiceDetails.Count > 0)
                    {
                        selectedService.ShowTime = svc.ServiceDetails[0].Description;
                    }

                    if (svc.SeatingDetails != null)
                    {
                        selectedService.BookedSeats = new List<BookedSeat>();
                        foreach (var seating in svc.SeatingDetails)
                        {
                            var bookedSeat = new BookedSeat
                            {
                                Row = seating.Row,
                                Seat = seating.Seat,
                                Gate = seating.Gate,
                                EntranceDoor = seating.EntranceDoor
                            };

                            selectedService.BookedSeats.Add(bookedSeat);
                        }
                    }

                    if (svc.VoucherDetails != null && svc.VoucherDetails.Count > 0)
                    {
                        selectedService.BookingVouchers = new List<BookingVoucher>();
                        foreach (var voucher in svc.VoucherDetails)
                        {
                            if (voucher.MimeType.Equals("application/pdf"))
                            {
                                var bookingVoucher = new BookingVoucher
                                {
                                    Language = voucher.LanguageCode,
                                    Url = voucher.Url,
                                    Type = BookingVoucherType.PDF
                                };

                                selectedService.BookingVouchers.Add(bookingVoucher);
                            }
                        }
                    }

                    purchaseItems.Add(selectedService);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "HotelBeds.PurchaseConfirmConverter",
                        MethodName = "ConvertPurchaseResult"
                    };
                    _logger.Error(isangoErrorEntity, ex);
                    throw; //use throw as existing flow should not break bcoz of logging implementation.
                }
            }
            return purchaseItems;
        }
    }
}