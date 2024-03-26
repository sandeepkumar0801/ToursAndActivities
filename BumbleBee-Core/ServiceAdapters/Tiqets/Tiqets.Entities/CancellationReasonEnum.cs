using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Tiqets.Tiqets.Entities
{
    public enum CancellationReasonEnum
    {
        COULD_NOT_MAKE_IT = 0,
        NEED_TO_RESCHEDULE = 1,
        FOUND_A_BETTER_PRICE_ELSEWHERE = 2,
        DID_NOT_RECEIVE_TICKETS = 3,
        OTHER = 4,
    }
}
