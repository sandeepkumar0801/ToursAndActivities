using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Canocalization;
using Isango.Entities.Enums;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters.Contracts
{
    class ReservationConverter : ConverterBase, IReservationConverter
    {
        public override object Convert(object objectResult)
        {
            throw new NotImplementedException();
        }
        public override object Convert(object objectResult,object objectResultDetail, object input)
        {
            throw new NotImplementedException();
        }

        public override object Convert(object objectResult, object input)
        {
            return null;
        }
    }
}
