using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.Rayna;
using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaAvailabilityCmdHandler : CommandHandlerBase, IRaynaAvailabilityCmdHandler
    {
        public RaynaAvailabilityCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var data = criteria as RaynaCriteria;
            var noOfAdult = 1;
            var noOfChild = 0;
            var noOfInfant = 0;
            if (((Criteria)data).NoOfPassengers != null)
            {
                var criteriaNoOfPassengers = ((Criteria)data)?.NoOfPassengers;
                noOfAdult = criteriaNoOfPassengers.Where(x => x.Key.ToString()?.ToLowerInvariant() == PassengerType.Adult.ToString()?.ToLowerInvariant()).FirstOrDefault().Value;
                noOfChild = criteriaNoOfPassengers.Where(x => x.Key.ToString()?.ToLowerInvariant() == PassengerType.Child.ToString()?.ToLowerInvariant()).FirstOrDefault().Value;
                noOfInfant = criteriaNoOfPassengers.Where(x => x.Key.ToString()?.ToLowerInvariant() == PassengerType.Infant.ToString()?.ToLowerInvariant()).FirstOrDefault().Value;
            }
            var returnData = new AvailabilityREQ
            {
                TourId = data.TourId,
                TourOptionId= data.TourOptionId,
                TransferId= data.TransferId,
                ContractId = Convert.ToInt32(data.ModalityCode),
                TravelDate = data.PassDate.ToString("MM/dd/yyyy"),
                Adult = noOfAdult,
                Child = noOfChild,
                Infant = noOfInfant,
            };
            return returnData;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(AvailabilityRES);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<AvailabilityRES>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override object GetResults(object input)
        {
            var data = input as AvailabilityREQ;
            var url = string.Format($"{_raynaURL}{Constants.Constants.AvailabilityTour}");
            var response =  GetResponseFromAPIEndPoint(data, url);
            return response;
        }
    }
}