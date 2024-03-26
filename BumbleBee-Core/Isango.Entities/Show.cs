using Isango.Entities.Activities;
using Isango.Entities.TheatreShow;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class Show : Activity
    {
        public Venue Venue { get; set; }

        public decimal SelectedHotelPrice
        {
            get;
            set;
        }

        public List<Product> ShowHotels
        {
            get; set;
        }
    }
}