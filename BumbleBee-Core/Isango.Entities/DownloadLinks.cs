using System;

namespace Isango.Entities
{
    public class DownloadLinks
    {
        public int ServiceId { get; set; }
        public string DownloadLink { get; set; }
        public string DownloadText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int DownloadId { get; set; }
    }
}