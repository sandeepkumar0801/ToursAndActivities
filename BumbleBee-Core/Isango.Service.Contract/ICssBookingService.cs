using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface ICssBookingService
    {
        void ProcessCssIncompleteBooking();
        void ProcessIncompleteCancellation();
        void ProcessIncompleteRedemption();

    }
}
