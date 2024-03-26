using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.ConsoleApplication.ServiceAvailability
{
    public class ExtraDetailQuestions
    {
        public int ActivityId { get; set; }
        public int OptionId { get; set; }

        public string Variant { get; set; }

        public TimeSpan StartTime { get; set; }

        public List<Isango.Entities.GoogleMaps.Question> Questions { get; set; }

        public List<int> PickUpLocationID { get; set; }

        public List<string> PickUpLocationName { get; set; }

        public List<int> DropOffLocationID { get; set; }

        public List<string> DropOffLocationName { get; set; }
    }

    //public class ExtraDetailPickUpLocations
    //{
    //    public int ActivityId { get; set; }
    //    public int OptionId { get; set; }

    //    public string Variant { get; set; }

    //    public TimeSpan StartTime { get; set; }

        
    //}

    //public class ExtraDetailDropOffLocations
    //{
    //    public int ActivityId { get; set; }
    //    public int OptionId { get; set; }

    //    public string Variant { get; set; }

    //    public TimeSpan StartTime { get; set; }

        
    //}

}
