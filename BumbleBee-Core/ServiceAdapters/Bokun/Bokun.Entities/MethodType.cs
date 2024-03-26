using System;

namespace ServiceAdapters.Bokun.Bokun.Entities
{
    public enum MethodType
    {
        Undefined = 0,
        Checkavailabilities = 1,
        Checkoutoptions = 2,
        Submitcheckout = 3,
        Getactivitybookingdetails = 4,
        Cancelbooking = 5,
        Editbooking = 6,
        Getbooking = 7,
        Getactivity = 8,
        Getpickupplaces = 9
    }

    [Serializable]
    public enum APIHitErrors
    {
        ErrorWhileAPIHit = 0
    }
}