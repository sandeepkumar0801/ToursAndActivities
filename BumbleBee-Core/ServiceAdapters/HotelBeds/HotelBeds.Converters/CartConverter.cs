using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Entities.Region;
using Logger.Contract;
using ServiceAdapters.HotelBeds.HotelBeds.Converters.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceAdapters.HotelBeds.HotelBeds.Converters
{
    public class CartConverter : ConverterBase, ICartConverter
    {
        public CartConverter(ILogger logger) : base(logger)
        {
        }

        public object Convert(object objectresult)
        {
            var serviceAddRs = (ServiceAddRs)objectresult;

            if (objectresult != null)
            {
                return ConvertCartResult(serviceAddRs);
            }

            return null;
        }

        private List<HotelBedsSelectedProduct> ConvertCartResult(ServiceAddRs cartResult)
        {
            var selectedServices = new List<HotelBedsSelectedProduct>();
            if (cartResult == null) return selectedServices;

            var hotelBedsSelectedProducts = (List<HotelBedsSelectedProduct>)cartResult?.InputCritera;
            var savedSelectedProduct = hotelBedsSelectedProducts?.FirstOrDefault();
            var savedSelectedProductOption = savedSelectedProduct?.ProductOptions?.FirstOrDefault();

            foreach (var svc in cartResult.Purchase.ServiceList)
            {
                var customers = new List<Customer>();
                var travelInfo = new TravelInfo();

                #region Reusing saved customer and travel info as is does not change after check avail

                var supplierOptionCodes = savedSelectedProduct?.ProductOptions?.Select(x => new { OptionCode = x.SupplierOptionCode?.Split('~')?.FirstOrDefault(), DestCode = x.SupplierOptionCode?.Split('~')?.LastOrDefault(), OriginalSupplierOptionCode = x.SupplierOptionCode })?.ToList();
                var isMatch = supplierOptionCodes.Any(x => svc.TicketInfo.Code == x.OptionCode && svc.TicketInfo.Destination.DestinationCode == x.DestCode);
                if (isMatch)
                {
                    var query = from p in hotelBedsSelectedProducts
                                from po in p?.ProductOptions
                                from spc in supplierOptionCodes
                                where po.SupplierOptionCode == spc.OriginalSupplierOptionCode
                                select po;
                    savedSelectedProductOption = query.FirstOrDefault();
                    if (savedSelectedProductOption != null)
                    {
                        travelInfo = savedSelectedProductOption.TravelInfo;
                        customers = savedSelectedProductOption.Customers;
                    }
                }

                #endregion Reusing saved customer and travel info as is does not change after check avail

                var selectedService = new HotelBedsSelectedProduct
                {
                    EchoToken = cartResult.EchoToken,
                    PurchaseToken = cartResult.Purchase.PurchaseToken,
                    ProductOptions = new List<ProductOption>()
                };
                var productOptions = new List<ProductOption>();

                #region Trivial Data

                var timeWindow = DateTime.Now;
                var ttE = timeWindow.AddMilliseconds(cartResult.Purchase.TimeToExpiration);
                selectedService.TimeToExpiration = ttE;
                selectedService.CartStatus = cartResult.Purchase.Status;

                #endregion Trivial Data

                selectedService.SPUI = svc.Spui;
                selectedService.ServiceStatus = svc.Status;
                selectedService.VatNumber = svc.Supplier.VatNumber;
                selectedService.SupplierName = svc.Supplier.Name;
                selectedService.Price = svc.TotalAmount;
                selectedService.SupplierCurrency = svc.Currency;

                if (selectedService.Regions == null)
                    selectedService.Regions = new List<Region>();

                selectedService.Name = svc.TicketInfo.Name;
                selectedService.Code = svc.TicketInfo.Code;
                selectedService.ProductType = ProductType.Activity;
                if (svc.TicketInfo.Destination != null)
                {
                    var region = new Region
                    {
                        Name = svc.TicketInfo.Destination.DestinationName
                    };
                    selectedService.Regions.Add(region);
                }

                customers = savedSelectedProductOption.Customers;
                var customerQuery = from c in savedSelectedProductOption?.Customers
                                    from g in svc?.PassengerDetails?.GuestList?.CustomerList
                                    where c.Age == g.Age
                                    && c.FirstName == g.Name
                                    && c.LastName == g.LastName
                                    select new Customer
                                    {
                                        FirstName = g.Name,
                                        LastName = g.LastName,
                                        CustomerId = g.Id,
                                        Age = g.Age,
                                        PassengerType = c.PassengerType,
                                        Email = c.Email,
                                        ChildDob = c.ChildDob,
                                        IsLeadCustomer = c.IsLeadCustomer,
                                        Title = c.Title
                                    };
                customers = customerQuery?.ToList();

                travelInfo.StartDate = svc.DateFrom;
                var difference = svc.DateTo - svc.DateFrom;
                travelInfo.NumberOfNights = difference.Days;

                var actOption = new ActivityOption();

                #region ActivityOption

                actOption.TravelInfo = travelInfo;
                actOption.Customers = customers;
                actOption.Name = svc.AvailableModality.Name;
                actOption.IsSelected = true;

                //Price and Currency
                var actPrice = new Price
                {
                    Amount = svc.TotalAmount,
                    Currency = new Currency
                    {
                        Name = svc.Currency,
                        IsoCode = cartResult.Purchase.Currency
                    }
                };
                actOption.BasePrice = actPrice;

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

                foreach (var comm in svc.Comments)
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

                selectedService.ProductOptions.AddRange(productOptions);

                selectedServices.Add(selectedService);
            }
            return selectedServices;
        }
    }
}