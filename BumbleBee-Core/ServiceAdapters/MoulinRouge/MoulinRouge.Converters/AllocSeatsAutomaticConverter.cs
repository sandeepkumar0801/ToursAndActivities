using Logger.Contract;
using ServiceAdapters.MoulinRouge.MoulinRouge.Converters.Contracts;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities;
using System.Collections.Generic;
using System.Linq;
using AllocSeatsAutomatic = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.AllocSeatsAutomatic;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Converters
{
    public class AllocSeatsAutomaticConverter : ConverterBase, IAllocSeatsAutomaticConverter
    {
        public AllocSeatsAutomaticConverter(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// Return Converted object by converting Response from API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        public override object Convert<T>(T inputContext)
        {
            var response = inputContext as AllocSeatsAutomatic.Response;
            var convertedResult = ConvertToResult(response);
            return convertedResult;
        }

        private List<AllocatedSeat> ConvertToResult(AllocSeatsAutomatic.Response response)
        {
            var resp = response.Body.ACP_AllocSeatsAutomaticRequestResponse;
            if (!resp.ACP_AllocSeatsAutomaticRequestResult || resp.result != 0)
                return new List<AllocatedSeat>();

            var query = from las in resp.listAllocResponse.ACPO_AllocSAResponse.ListAllocSeat
                        select new AllocatedSeat
                        {
                            TemporaryOrderId = resp.id_TemporaryOrder,
                            IsContiguous = resp.isContiguous,
                            TemporaryOrderRowId = resp.listAllocResponse.ACPO_AllocSAResponse.ID_TemporaryOrderRow,
                            Amount = las.Amount,
                            IdentityId = las.ID_Identity,
                            RuleId = las.ID_Rule,
                            AmountDetail = las.AmountDetail,
                            AccessId = las.Seat_Detail.ID_Access,
                            DesignationId = las.Seat_Detail.ID_Designation,
                            DoorId = las.Seat_Detail.ID_Door,

                            PhotoSeatId = las.Seat_Detail.ID_PhotoSeat,
                            PhysicalSeatId = las.Seat_Detail.ID_PhysicalSeat,
                            SeatId = las.Seat_Detail.ID_Seat,
                            TribuneId = las.Seat_Detail.ID_Tribune,
                            VenueId = las.Seat_Detail.ID_Venue,
                            IsNumbered = las.Seat_Detail.isNumbered,
                            Rank = las.Seat_Detail.Rank,
                            Rotation = las.Seat_Detail.Rotation,
                            Seat = las.Seat_Detail.Seat,
                            Status = las.Seat_Detail.Status,
                            TypeSeat = las.Seat_Detail.Type,
                            SeatX = las.Seat_Detail.X,
                            SeatY = las.Seat_Detail.Y
                        };
            var recordSetForSeat = query.ToList();

            return recordSetForSeat;
        }
    }
}