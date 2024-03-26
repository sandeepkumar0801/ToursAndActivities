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
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.HotelBeds.HotelBeds.Converters
{
    public class PurchaseDetailsConverter : ConverterBase, IPurchaseDetailsConverter
    {
        public PurchaseDetailsConverter(ILogger logger) : base(logger)
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

            var productOptions = new List<ProductOption>();
            foreach (var svc in purchaseData.Purchase.ServiceList)
            {
                var selectedService = new HotelBedsSelectedProduct
                {
                    EchoToken = purchaseData.EchoToken,
                    PurchaseToken = purchaseData.Purchase.PurchaseToken,
                    TimeToExpiration = DateTime.Now.AddMilliseconds(purchaseData.Purchase.TimeToExpiration),
                    CartStatus = purchaseData.Purchase.Status,
                    FileNumber = svc.FileNumber,
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

                selectedService.Name = svc.TicketInfo.Name;
                selectedService.Code = svc.TicketInfo.Code;
                var region = new Region { Name = svc.TicketInfo.Destination.DestinationName };
                selectedService.Regions.Add(region);

                // Travel Info
                var travelInfo = new TravelInfo();

                travelInfo.NoOfPassengers.Add(PassengerType.Adult, svc.PassengerDetails.AdultCount);
                travelInfo.NoOfPassengers.Add(PassengerType.Child, svc.PassengerDetails.ChildCount);

                var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
                var children = svc.PassengerDetails.GuestList.CustomerList.FindAll(g => g.Type.Equals(Constant.Ch));

                Parallel.ForEach(children, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, child => 
                {
                    travelInfo.Ages.Add(PassengerType.Child, child.Age);
                });

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

                var actOption = new ActivityOption();

                #region ActivityOption

                actOption.Name = svc.AvailableModality.Name;
                actOption.IsSelected = true;
                actOption.TravelInfo = travelInfo;
                actOption.Customers = customers;

                //Price and Currency
                var actPrice = new Price
                {
                    Amount = svc.TotalAmount,
                    Currency = new Currency { Name = svc.Currency, IsoCode = purchaseData.Purchase.Currency }
                };
                actPrice.Amount = svc.TotalAmount;
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

                if (svc.CancellationCharges != null)
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

                productOptions.Add(actOption);
                //selectedService.OccupancyUnits.Add(unit);
                purchaseItems.Add(selectedService);
            }
            return purchaseItems;
        }
    }
}