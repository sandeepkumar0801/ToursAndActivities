using System;

namespace Isango.Entities.Review
{
    public class Review
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public string Country { get; set; }
        public string Rating { get; set; }
        public string ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int ServiceReviewsCount { get; set; }
        public string ImageName { get; set; }
        public int ImageId { get; set; }
        public DateTime SubmittedDate { get; set; }
        public bool IsFeefo { get; set; }
    }
}