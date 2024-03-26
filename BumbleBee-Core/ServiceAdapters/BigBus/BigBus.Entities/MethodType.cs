using System;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    [Serializable]
    public enum MethodType
    {
        CreateReservation = 1,
        CancelReservation = 2,
        CreateBooking = 3,
        CancelBooking = 4
    }
}