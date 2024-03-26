using System;

namespace Isango.Entities.HotelBeds
{
    public class ApiTudeAgeGroup
    {
        public Int32 FromAge { get; set; }
        public Int32 ToAge { get; set; }

        public string AgeType { get; set; }

        public string ServiceCode { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }

        public string ModalityCode { get; set; }

        public Int32 ServiceID { get; set; }

        public string FactsheetID { get; set; }
    }
}