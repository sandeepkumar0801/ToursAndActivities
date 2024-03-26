using Isango.Entities;
using Isango.Entities.Aot;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.Aot.Aot.Commands.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System.Text;
using Constant = ServiceAdapters.Aot.Constants.Constant;

namespace ServiceAdapters.Aot.Aot.Commands
{
    public class CreateBookingCommandHandler : CommandHandlerBase, ICreateBookingCommandHandler
    {
        public CreateBookingCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object AotApiRequest<T>(T inputContext)
        {
            var createBooking = inputContext as AddBookingRequest;
            if (createBooking == null) return null;

            createBooking.AgentId = AgentId;
            createBooking.Password = Password;

            var bookingXML = SerializeXml(createBooking).Replace("<Age>0</Age>", string.Empty);
            var content = new StringContent(bookingXML, Encoding.UTF8, Constant.ApplicationMediaType);

            var result = HttpClient.PostAsync(string.Empty, content);
            result.Wait();
            return result.Result.Content.ReadAsStringAsync().Result;
        }

        protected override async Task<object> AotApiRequestAsync<T>(T inputContext)
        {
            var createBooking = inputContext as AddBookingRequest;
            if (createBooking == null) return null;

            createBooking.AgentId = AgentId;
            createBooking.Password = Password;

            var bookingXml = SerializeXml(createBooking).Replace("<Age>0</Age>", string.Empty);
            var content = new StringContent(bookingXml, Encoding.UTF8, Constant.ApplicationMediaType);
            var result = await HttpClient.PostAsync(string.Empty, content);
            return result.Content.ReadAsStringAsync().Result;
        }

        protected override object CreateInputRequest<T>(T inputContext, string referenceNumber)
        {
            var selectedProducts = inputContext as List<SelectedProduct>;
            var request = CreateBookingRequest(selectedProducts, referenceNumber);
            return request;
        }

        private AddBookingRequest CreateBookingRequest(List<SelectedProduct> selectedProducts, string referenceNumber)
        {
            var services = new Services();
            var listAddServiceInfo = new List<AddServiceInfo>();

            foreach (var item in selectedProducts)
            {
                var selectedProduct = (AotSelectedProduct)item;
                var paxDetails = new List<PaxDetails>();
                var customerList = selectedProduct.ProductOptions
                                  .Find(x => x.IsSelected.Equals(true))?
                                  .Customers;

                if (customerList?.Count > 0)
                {
                    int i = 1;
                    foreach (var customerDetails in customerList)
                    {
                        var paxDetail = new PaxDetails
                        {
                            PaxType = customerDetails.PassengerType.Equals(PassengerType.Adult) ? "A" :
                       customerDetails.PassengerType.Equals(PassengerType.Child) ? "C" : string.Empty,
                            Forename = customerDetails.FirstName,
                            Surname = customerDetails.LastName,
                            Title = customerDetails.Title,
                            Age = customerDetails.PassengerType.Equals(PassengerType.Adult) ? 0 : customerDetails.Age
                        };

                        if (paxDetail?.PaxType != string.Empty)
                        {
                            //Add counter to have unique name
                            if (paxDetails.Any(x => x.Forename == paxDetail.Forename && x.Surname == paxDetail.Surname))
                            {
                                paxDetail.Surname += i.ToString();
                                i++;
                            }
                            paxDetails.Add(paxDetail);
                        }
                    }
                    //paxDetails.AddRange(customerList.Select(customerDetails => new PaxDetails
                    //{
                    //    PaxType = customerDetails.PassengerType.Equals(PassengerType.Adult) ? "A" :
                    //    customerDetails.PassengerType.Equals(PassengerType.Child) ? "C" : string.Empty,
                    //    Forename = customerDetails.FirstName,
                    //    Surname = customerDetails.LastName,
                    //    Title = customerDetails.Title,
                    //    Age = customerDetails.PassengerType.Equals(PassengerType.Adult) ? 0 : customerDetails.Age
                    //}).Where(x => x.PaxType != string.Empty));
                }
                var serviceInfo = GetServiceInfo(selectedProduct, paxDetails);
                listAddServiceInfo.Add(serviceInfo);
            }

            services.AddServiceInfo = listAddServiceInfo;
            var customer = selectedProducts[0].ProductOptions.Find(x => x.IsSelected.Equals(true))?.Customers[0];

            var contactDetails = new ContactDetails
            {
                Forename = customer?.FirstName,
                Surname = customer?.LastName,
                Email = customer?.Email
            };

            var request = new AddBookingRequest
            {
                AgentId = AgentId,
                Password = Password,
                EmailNotification = "true",
                Name = contactDetails.Surname?.ToUpper() + "/" + contactDetails.Forename?.Substring(0, 1).ToUpper(),
                ContactDetails = contactDetails,
                Services = services,
                AgentRef = referenceNumber
            };
            return request;
        }

        private AddServiceInfo GetServiceInfo(AotSelectedProduct selectedProduct, List<PaxDetails> paxDetails)
        {
            var extraQuantities = new ExtraQuantities();
            var addServiceInfo = new AddServiceInfo
            {
                Opt = selectedProduct.OptCode,
                DateFrom = selectedProduct.ProductOptions.Find(x => x.IsSelected.Equals(true)).TravelInfo.StartDate.ToString(Constant.DateFormatmmddyyhipen),
                ExtraQuantities = extraQuantities,
                ScUqty = "1",
                RoomConfigs = GetRoomConfig(selectedProduct, paxDetails),
                PuRemark = !string.IsNullOrEmpty(selectedProduct.PuRemark) ? selectedProduct.PuRemark : string.Empty,
                PuTime = !string.IsNullOrEmpty(selectedProduct.PuTime) ? selectedProduct.PuTime : "00:00:00",
                Comments = selectedProduct.SpecialRequest
            };
            return addServiceInfo;
        }

        private RoomConfigs GetRoomConfig(AotSelectedProduct selectedProduct, List<PaxDetails> paxDetails)
        {
            var travelInfo = selectedProduct.ProductOptions.FirstOrDefault(prod => prod.IsSelected)?.TravelInfo;
            var roomConfigs = new RoomConfigs
            {
                RoomConfig = new List<RoomConfig> {
                    new RoomConfig
                    {
                        Adults = travelInfo?.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Adult).Value.ToString(),
                        Children = travelInfo?.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Child).Value.ToString(),
                        //Infants = travelInfo?.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Infant).Value.ToString(),
                        RoomType = selectedProduct.AotOptionType ==Constant.RoomBased  ? selectedProduct.RoomType : string.Empty,
                        PaxList = new PaxList
                        {
                            PaxDetails = paxDetails
                        }
                    }
                }
            };
            return roomConfigs;
        }
    }
}