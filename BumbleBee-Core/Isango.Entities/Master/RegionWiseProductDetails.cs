using System;

namespace Isango.Entities.Master
{
    public class RegionWiseProductDetails
    {
        public Int32 RegionID { get; set; }
        public string RegionName { get; set; }

        public Int32 ActivityID { get; set; }
        public string ActivityName { get; set; }
        public Int32 FromAge { get; set; }
        public Int32 ToAge { get; set; }
        public Int32 MinSize { get; set; }
        public Int32 MaxSize { get; set; }
        public string PassengerTypeName { get; set; }

        public Int32 PassengerTypeID { get; set; }
        public string Label { get; set; }
    }
}