using System.Collections.Generic;

namespace Isango.Entities.Review
{
    public class RegionReviews
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Review> Reviews { get; set; }
        public string Url { get; set; }
    }
}