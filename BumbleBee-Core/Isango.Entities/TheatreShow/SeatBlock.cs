using System.Collections.Generic;

namespace Isango.Entities.TheatreShow
{
    public class SeatBlock
    {
        public List<Seat> Seats { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Maps to Activity.Option.Name
        /// </summary>
        public bool IsSelected { get; set; }

        public SeatingSession SeatingSession { get; set; }
        public string Id { get; set; }
    }

    public enum SeatingSession
    {
        Undefined = 0,
        Evening = 1,
        Matinee = 2,
        All = 3
    }

    public enum ShowType
    {
        Undefined = 0,
        Musical = 1,
        Opera = 2,
        Play = 3
    }
}