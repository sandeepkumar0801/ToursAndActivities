using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Ventrata.Ventrata.Entities
{
    public enum MethodType
    {
        Availability = 1,
        BookingReservation = 2,
        CreateBooking = 3,
        CancelReservationAndBooking = 4,
        GetAllProducts = 5,
        CustomQuestions = 6
    }
}
