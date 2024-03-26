using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.HotelBeds;
using Isango.Entities.Region;
using Logger.Contract;
using ServiceAdapters.HotelBeds.HotelBeds.Converters.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets;
using System.Collections.Generic;
using System.Linq;

namespace ServiceAdapters.HotelBeds.HotelBeds.Converters
{
    public class TicketValuationConverter : ConverterBase, ITicketValuationConverter
    {
        public TicketValuationConverter(ILogger logger) : base(logger)
        {
        }

        public object Convert(object objectresult)
        {
            var serviceAddRs = (TicketValuationRs)objectresult;
            if (objectresult != null)
            {
                return ConvertCartResult(serviceAddRs);
            }

            return null;
        }

        private HotelBedsSelectedProduct ConvertCartResult(TicketValuationRs cartResult)
        {
            var selectedService = new HotelBedsSelectedProduct();
            if (cartResult == null) return selectedService;

            var customers = cartResult?.HotelBedsSelectedProduct?.ProductOptions[0]?.Customers;
            var travelInfo = cartResult?.HotelBedsSelectedProduct?.ProductOptions[0]?.TravelInfo;

            #region Trivial Data

            selectedService.EchoToken = cartResult?.EchoToken;
            selectedService.SPUI = cartResult?.ServiceTicket.Spui;
            selectedService.ServiceStatus = cartResult?.ServiceTicket.Status;
            selectedService.SupplierCurrency = cartResult?.ServiceTicket.Currency;

            #endregion Trivial Data

            if (selectedService.Regions == null)
                selectedService.Regions = new List<Region>();

            var productOptions = new List<ProductOption>();

            #region Travel Info and customer details are already populated from check avail so reusing that.

            /*
               //Add Customers
               var customers = cartResult.ServiceTicket.PassengerDetails.GuestList.CustomerList.Select(guest => new Isango.Entities.Customer
               {
                   Age = guest.Age,
                   PassengerType = (guest.Type.Equals(Constant.Ad) ? PassengerType.Adult : PassengerType.Child)
               })
                   .ToList();

               // Travel Info
               var travelInfo = new TravelInfo
               {
                   NoOfPassengers = new Dictionary<PassengerType, int>(),
                   Ages = new Dictionary<PassengerType, int>()
               };

               travelInfo.NoOfPassengers.Add(PassengerType.Adult, cartResult.ServiceTicket.PassengerDetails.AdultCount);
               travelInfo.NoOfPassengers.Add(PassengerType.Child, cartResult.ServiceTicket.PassengerDetails.ChildCount);

               if (cartResult.ServiceTicket.PassengerDetails.ChildCount > 0)
               {
                   var childAges = (from c in customers where (c.PassengerType == PassengerType.Child) select c.Age).Distinct().ToList();
                   if (childAges?.Count > 0)
                   {
                       //Parallel.ForEach(childAges, childAge =>
                       //        {
                       //            travelInfo.Ages.Add(PassengerType.Child, childAge);
                       //        });
                       foreach (var childAge in childAges)
                       {
                           var customer = savedCustomers.FirstOrDefault(x => x.Age == childAge);
                           if (customer!=null && !travelInfo.Ages.Keys.Contains(customer.PassengerType))
                               travelInfo.Ages.Add(customer.PassengerType, childAge);
                       }
                   }
               }
               */

            #endregion Travel Info and customer details are already populated from check avail so reusing that.

            // ReSharper disable once PossibleNullReferenceException
            travelInfo.StartDate = cartResult.ServiceTicket.DateFrom;
            var difference = cartResult.ServiceTicket.DateTo - cartResult.ServiceTicket.DateFrom;
            travelInfo.NumberOfNights = difference.Days;

            if (cartResult.ServiceTicket.ServiceDetails != null)
            {
                var contractQuestions = cartResult.ServiceTicket.ServiceDetails.Select(item => new ContractQuestion { Code = item.Code, Name = item.Name }).ToList();
                selectedService.ContractQuestions = contractQuestions;
            }

            var actOption = new ActivityOption
            {
                Name = cartResult.ServiceTicket.AvailableModality.Name,
                IsSelected = true,
                TravelInfo = travelInfo,
                Customers = customers
            };

            #region ActivityOption

            //Price and Currency
            var actPrice = new Price
            {
                Amount = cartResult.ServiceTicket.TotalAmount,
                Currency = new Currency
                {
                    Name = cartResult.ServiceTicket.Currency,
                    IsoCode = cartResult.ServiceTicket.Currency
                }
            };
            actOption.BasePrice = actPrice;

            //Add Contract

            #region HB Contract question and comments

            var comments = new List<Comment>();
            if (cartResult?.ServiceTicket?.Contract?.Comments != null)
            {
                foreach (var item in cartResult?.ServiceTicket?.Contract?.Comments)
                {
                    var comment = new Comment
                    {
                        CommentText = item?.CommentText,
                        Type = item?.Type
                    };
                    comments.Add(comment);
                }
            }
            var contract = cartResult?.ServiceTicket?.Contract;
            if (contract != null)
            {
                selectedService.ServiceContract = new Isango.Entities.HotelBeds.Contract
                {
                    Name = contract.Name,
                    ClassificationCode = contract.Classification,
                    Comments = comments,
                    InComingOfficeCode = contract.IncomingOffice,
                    Classification = contract.Classification
                };
            }

            #endregion HB Contract question and comments

            //var actComments = new List<Isango.Entities.HotelBeds.Comment>();

            //actOption.Contract = new Isango.Entities.Contract
            //{
            //    Name = cartResult.ServiceTicket.Contract.Name,
            //    ClassificationCode = cartResult.ServiceTicket.Contract.Classification
            //};

            //foreach (var comm in cartResult.ServiceTicket.Contract.Comments)
            //{
            //    var comment = new Isango.Entities.HotelBeds.Comment { CommentText = comm.CommentText, Type = comm.Type };
            //    actComments.Add(comment);
            //}
            //selectedService.ServiceContract.Comments = actComments;
            //selectedService.ServiceContract.Classification = cartResult.ServiceTicket.Contract.Classification;
            //selectedService.ServiceContract.InComingOfficeCode = cartResult.ServiceTicket.Contract.IncomingOffice;

            actOption.Contract = new Isango.Entities.Contract
            {
                Name = contract?.Name,
                ClassificationCode = contract?.Classification,
                Comments = comments,
                InComingOfficeCode = contract?.IncomingOffice,
                Classification = contract?.Classification
            };

            if (cartResult.ServiceTicket.CancellationCharges != null)
            {
                var cancellationCost = new List<CancellationPrice>();
                foreach (var charge in cartResult.ServiceTicket.CancellationCharges)
                {
                    var price = new CancellationPrice
                    {
                        CancellationToDate = charge.ToDate,
                        CancellationFromdate = charge.FromDate,
                        CancellationAmount = charge.Amount
                    };
                    cancellationCost.Add(price);
                }
                actOption.CancellationPrices = cancellationCost;
            }

            #endregion ActivityOption

            productOptions.Add(actOption);
            selectedService.ProductOptions = productOptions;
            return selectedService;
        }
    }
}