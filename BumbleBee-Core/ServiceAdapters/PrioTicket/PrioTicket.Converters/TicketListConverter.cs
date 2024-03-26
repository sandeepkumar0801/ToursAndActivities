using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.Prio;
using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Converters.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Collections.Generic;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters
{
    public class TicketListConverter : ConverterBase, ITicketListConverter
    {
        public TicketListConverter(ILogger logger) : base(logger)
        {
        }
        public override object Convert(object objectResult)
        {
            return ConvertTicketList(objectResult);
        }

        /// <summary>
        /// Conver Ticket Result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private TicketListRs ConvertTicketList(object result)
        {
            return (TicketListRs)result;
        }
    }
}