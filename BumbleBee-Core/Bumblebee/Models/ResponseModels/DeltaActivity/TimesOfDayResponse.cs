using Isango.Entities.Activities;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels.DeltaActivity
{
    public class TimesOfDayResponse
    {
        public int ServiceOptionID { get; set; }
        public List<TimesOfDay> TimesOfDay { get; set; }
    }
}