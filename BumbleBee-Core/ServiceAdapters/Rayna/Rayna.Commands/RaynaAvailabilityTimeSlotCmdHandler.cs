using Isango.Entities.Rayna;
using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna.Rayna.Commands
{
    public class RaynaAvailabilityTimeSlotCmdHandler : CommandHandlerBase, IRaynaAvailabilityTimeSlotCmdHandler
    {
        public RaynaAvailabilityTimeSlotCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var data = criteria as RaynaCriteria;

            var returnData = new AvailabilityTimeSlotRQ
            {
                TourId = data.TourId,
                TransferId=data.TransferId,
                TourOptionId= data.TourOptionId,
                ContractId = Convert.ToInt32(data.ModalityCode),
                TravelDate = data.PassDate.ToString("yyyy-MM-dd")               
            };
            return returnData;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(AvailabilityTimeSlotRS);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<AvailabilityTimeSlotRS>(responseText);
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
            var data = input as AvailabilityTimeSlotRQ;
            var url = string.Format($"{_raynaURL}{Constants.Constants.AvailabilityTimeSlots}");
            var response =  GetResponseFromAPIEndPoint(data, url);
            return response;
        }
    }
}