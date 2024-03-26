using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Rayna.Constants
{
    public sealed class Constants
    {
        public const string RequestPrepare = "Request Prepare, it should change";
        public const string RaynaApi = "Rayna";
        public const string Token = "Token";

        public const string RaynaURL = "RaynaURL";
        public const string RaynaToken = "RaynaToken";
        //Static Data Endpoints
        public const string Countries = "tour/countries";
        public const string Cities = "tour/cities";
        public const string TourStaticData = "tour/tourstaticdata";
        public const string TourStaticDataById = "tour/tourStaticDataById";
        public const string TourOptions = "tour/touroptionstaticdata";
        //Availability Endpoints
        public const string AvailabilityTourOptions = "tour/touroption";
        public const string AvailabilityTimeSlots = "tour/timeslot";
        public const string AvailabilityTour = "tour/availability";
        public const string Booking = "Booking/bookings";
        public const string GetBookedTickets = "Booking/GetBookedTickets";
        public const string Cancel = "Booking/cancelbooking";
        public const string Authorization = "Authorization";
    }
}
