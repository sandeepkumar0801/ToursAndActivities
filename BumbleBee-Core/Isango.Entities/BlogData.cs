using System;
using System.Collections.Generic;

namespace Isango.Entities
{
    [Serializable]
    public class BlogData
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Category { get; set; }
        public string ImageName { get; set; }
        public List<string> Destinations { get; set; }
    }

    [Serializable]
    public class BlogDestination
    {
        public string BlogId { get; set; }
        public string DestinationId { get; set; }
        public string DestinationName { get; set; }
    }
}