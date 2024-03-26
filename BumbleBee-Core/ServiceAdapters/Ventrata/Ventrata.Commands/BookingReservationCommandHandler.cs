using Logger.Contract;
using Newtonsoft.Json.Linq;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using ServiceAdapters.Ventrata.Ventrata.Entities.Request;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using Util;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;
using RequestVentrata = ServiceAdapters.Ventrata.Ventrata.Entities.Request;

namespace ServiceAdapters.Ventrata.Ventrata.Commands
{
    public class BookingReservationCommandHandler : CommandHandlerBase, IBookingReservationCommandHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly RequestVentrata.BookingReservationReq _reservationReq;
        private readonly string _isTestMode;


        public BookingReservationCommandHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient()
            {
                ServiceURL = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.VentrataAPIUrl)) + ConstantsForVentrata.BookingReservation
            };
            _reservationReq = new RequestVentrata.BookingReservationReq();
            _isTestMode = ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.IsRequestInTestMode);
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            _reservationReq.ProductId = inputContext.ProductId.Trim();
            _reservationReq.OptionId = inputContext.OptionCode.Trim();
            _reservationReq.AvailabilityId = inputContext.AvailabilityId;
            _reservationReq.Units = new List<RequestVentrata.Unit>();
          
            foreach (var unitId in inputContext.UnitIdsForBooking) {
                var unit = new RequestVentrata.Unit();
                unit.UnitId = unitId;
               _reservationReq.Units.Add(unit);
            }

            if (inputContext.pickUpRequested)
            {
                _reservationReq.IsPickUpRequested = inputContext.pickUpRequested;
                _reservationReq.PickUpPointId = inputContext.pickUpId;
            }
            else
            {
                _reservationReq.PickUpPointId = string.Empty;
            }

            //Handle Ages
            //age at paxlevel or root level
            var ageGet = inputContext.customers.Where(x=>x.AgeSupplier!="" && x.AgeSupplier !=null).ToList();

            if (ageGet != null && ageGet.Count() > 1)
            {
                foreach (var paxUnit in _reservationReq.Units)
                {
                    
                    var returnData = PaxWiseQuestions(paxUnit.UnitId, inputContext.customers);
                    if (returnData != null && returnData.Count > 0)
                    {
                        paxUnit.QuestionAnswers = new List<QuestionAnswer>();
                        paxUnit.QuestionAnswers = returnData;
                    }
                }
            }
            else
            {
                    var returnData = PaxWiseQuestions(inputContext.customers);
                    if (returnData != null && returnData.Count > 0)
                    {
                    _reservationReq.QuestionAnswers = new List<QuestionAnswer>();
                    _reservationReq.QuestionAnswers = returnData;
                    }
                
            }

            //Create headers required for this call
            Dictionary<string, string> HeadersToAddToRequest = new Dictionary<string, string>();
            HeadersToAddToRequest.Add(ConstantsForVentrata.AuthorizationHeaderKey, ConstantsForVentrata.Bearer + " " + inputContext.SupplierBearerToken);
            HeadersToAddToRequest.Add(ConstantsForVentrata.TestModeKey, _isTestMode);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoEnvKey, _isTestMode.ToLower() == "true" ? ConstantsForVentrata.OctoTestKey : ConstantsForVentrata.OctoLiveKey);
            HeadersToAddToRequest.Add(ConstantsForVentrata.OctoCapabilities, ConstantsForVentrata.OctoContentKey + ',' + ConstantsForVentrata.OctoPricingKey + ',' + ConstantsForVentrata.OctoPickupsKey + ',' + ConstantsForVentrata.OctoQuestionsKey + ',' + ConstantsForVentrata.OctoPackagesKey/* + ',' + ConstantsForVentrata.OctoOffersKey*/);

            var jsonRequest = SerializeDeSerializeHelper.Serialize(_reservationReq);
            return new Tuple<string, object, string>(jsonRequest, HeadersToAddToRequest,inputContext.VentrataBaseURL);
        }

        protected override object GetJsonResults(object requestObject, string token, string uuid)
        {
            var response = new HttpResponseMessage();
            var inputTupleOfJsonStringAndHeaders = requestObject as Tuple<string, object, string>;

            //Set Dynamic value from database;
            var baseURLGet = inputTupleOfJsonStringAndHeaders?.Item3;
            if (!string.IsNullOrEmpty(baseURLGet))
            {
                _asyncClient.ServiceURL = baseURLGet+ ConstantsForVentrata.BookingReservation;
            }

            var serializedRequest = inputTupleOfJsonStringAndHeaders.Item1;
            if (serializedRequest.Contains("questionAnswers"))
            {
                var removeEntities = JObject.Parse(serializedRequest);
                removeEntities.Descendants()
                .OfType<JProperty>()
                .Where(attr => (attr.Name == "questionAnswers" && attr.Value.ToString() == ""))
                .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                .ForEach(attr => attr.Remove()); // removing unwanted attributes

                serializedRequest = removeEntities.ToString();
            }
            

            var httpResponse = _asyncClient.PostJsonWithHeadersAsync(serializedRequest, (Dictionary<string, string>)inputTupleOfJsonStringAndHeaders.Item2);
            if (httpResponse != null)
            {
                response = httpResponse.GetAwaiter().GetResult();
                return response;
            }
            return response;
        }

        protected override object GetResults(object jsonResult)
        {
            return SerializeDeSerializeHelper.DeSerializeWithNullValueHandling<BookingReservationRes>(jsonResult.ToString());
        }

        private List<QuestionAnswer> PaxWiseQuestions(List<Isango.Entities.Customer> customers)
        {
            var questionAnswerLst = new List<QuestionAnswer>();
            var getAgeAllPax = customers?.Where(x => x.AgeSupplier != "" && x.AgeSupplier != null && x.IsLeadCustomer==true)?.ToList();
            var getAgePax = getAgeAllPax?.FirstOrDefault()?.AgeSupplier;

            var age = String.Empty;
            var questionId = String.Empty;
            if (!string.IsNullOrEmpty(getAgePax))
            {
                try
                {
                    var ageArray = getAgePax.Split(new string[] { "||" }, StringSplitOptions.None);
                    age = Convert.ToString(ageArray[0]);
                    questionId = Convert.ToString(ageArray[1].Split('_')[3]);
                }
                catch (Exception ex)
                {
                }
            }
            var questionAnswer = new QuestionAnswer
            {
                Value = age,
                QuestionId = questionId
            };
            if (questionAnswer != null)
            {
                if (!string.IsNullOrEmpty(questionAnswer.Value) && !string.IsNullOrEmpty(questionAnswer.QuestionId))
                {
                    questionAnswerLst.Add(questionAnswer);
                }
            }
            return questionAnswerLst;
        }

        private List<QuestionAnswer> PaxWiseQuestions(string pax ,List<Isango.Entities.Customer> customers)
        {
            var questionAnswerLst = new List<QuestionAnswer>();
            var getAgeAllPax = customers?.Where(x => x.PassengerType.ToString().ToLower() == pax);
            var getAgePax = getAgeAllPax?.FirstOrDefault()?.AgeSupplier;
            
            var age = String.Empty;
            var questionId = String.Empty;
            if (!string.IsNullOrEmpty(getAgePax))
            {
                try
                {
                    var ageArray = getAgePax.Split(new string[] { "||" }, StringSplitOptions.None);
                    age = Convert.ToString(ageArray[0]);
                    questionId = Convert.ToString(ageArray[1].Split('_')[3]);
                }
                catch(Exception ex)
                {
                }
            }
            var questionAnswer = new QuestionAnswer
            {
                Value = age,
                QuestionId = questionId
            };
            if (questionAnswer != null)
            {
                questionAnswerLst.Add(questionAnswer);
            }
            return questionAnswerLst;
        }
    }
}
