using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.Rayna;
using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaAvailabilityTourOptionCmdHandler : CommandHandlerBase, IRaynaAvailabilityTourOptionCmdHandler
    {
        public RaynaAvailabilityTourOptionCmdHandler(ILogger iLog) : base(iLog)
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
                noOfAdult = criteriaNoOfPassengers.Where(x=>x.Key.ToString()?.ToLowerInvariant()==PassengerType.Adult.ToString()?.ToLowerInvariant()).FirstOrDefault().Value;
                noOfChild = criteriaNoOfPassengers.Where(x => x.Key.ToString()?.ToLowerInvariant() == PassengerType.Child.ToString()?.ToLowerInvariant()).FirstOrDefault().Value; 
                noOfInfant = criteriaNoOfPassengers.Where(x => x.Key.ToString()?.ToLowerInvariant() == PassengerType.Infant.ToString()?.ToLowerInvariant()).FirstOrDefault().Value; 
            }

            var returnData = new AvailabilityTourOptionRQ
            {
                TourId = Convert.ToInt32(data.TourId),//tourid
                ContractId = Convert.ToInt32(data.ModalityCode),//contractid
                NoOfAdult= noOfAdult==0?1: noOfAdult,
                NoOfChild= noOfChild,
                NoOfInfant= noOfInfant,
                TravelDate= data.PassDate.ToString("MM/dd/yyyy")
            };
            return returnData;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(AvailabilityTourOptionRS);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<AvailabilityTourOptionRS>(responseText);
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
            var data = input as AvailabilityTourOptionRQ;
            var url = string.Format($"{_raynaURL}{Constants.Constants.AvailabilityTourOptions}");
            var response =  GetResponseFromAPIEndPoint(data, url);
            return response;
        }
    }
}