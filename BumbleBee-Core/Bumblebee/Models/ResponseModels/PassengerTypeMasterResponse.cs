using System;

namespace WebAPI.Models.ResponseModels.Master
{
    public class PassengerTypeMasterResponse
    {
        
        public string Name { get; set; }
        public Int32 minAge { get; set; }
        public Int32 maxAge { get; set; }
        public string Label { get; set; }
        public Int32 minSize { get; set; }
        public Int32 maxSize { get; set; }
        public Int32 passengerTypeID { get; set; }


    }
}