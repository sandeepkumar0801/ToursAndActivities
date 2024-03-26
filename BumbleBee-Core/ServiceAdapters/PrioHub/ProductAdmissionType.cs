namespace ServiceAdapters.PrioHub.PrioHub.Entities
{
    public enum ProductAdmissionType
    {
        TIMEPERIOD = 0,//TIME_PERIOD
        //Customers can arrive at any time between the start 
        //(availability_from_date_time) and end time (availability_to_date_time) 
        //of the availability slot. Multiple periods in a single day should be expected.
        //Therefore a date- and timepicker should be shown.
        TIMEDATE = 1,//TIME_DATE
        //Variation on TIME_PERIOD, whereas only a single period exists in a day.
        //It is not required to choose between different times within a day, therefore 
        //only a datepicker is required. Note that in case the slot includes midnight (two or more days), 
        //the day from which the availability_from_date_time originated should take precedence.
        TIMEPOINT = 2,//TIME_POINT
        // Customers are required to be present at the start time of the availability slot
        //but can leave any time they want.
        TIMESLOT = 3,//TIME_SLOT
        //Customers can arrive at any time. Availablity is not applicable.
        TIMEOPEN = 4,//TIME_OPEN
        //Customers are required to be present at the start time of the availability slot, and the 
        //service is expected to finish at the end time of the slot. 
    }
}