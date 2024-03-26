namespace Isango.Entities.Review
{
    public class UserReview
    {
        public Review ReviewData { get; set; }
        public int ReviewValue { get; set; }
        public int ActivityId { get; set; }
        public ReviewStatus Status { get; set; }
        public string BookingRef { get; set; }
        public string Email { get; set; }
    }
}