using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities.ConsoleApplication.ServiceAvailability
{
    public class Criteria
    {
        public List<IsangoHBProductMapping> MappedProducts { get; set; }

        public int Days2Fetch { get; set; }

        public int Months2Fetch { get; set; }

        public int Counter { get; set; }

        public bool SameDay { get; set; }

        public string Token { get; set; }

        public string Language { get; set; }
        public APIType ApiType { get; set; }

    }
}