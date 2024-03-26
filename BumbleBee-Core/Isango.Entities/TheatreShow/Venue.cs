using System.Collections.Generic;

namespace Isango.Entities.TheatreShow
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Map { get; set; }
        public Coordinates Coordinates { get; set; }
        public List<SeatBlock> Blocks { get; set; }
        public int RequiredSeats { get; set; }
    }
}