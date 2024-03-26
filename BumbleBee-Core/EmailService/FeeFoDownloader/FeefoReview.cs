using System;

namespace FeefoDownloader
{
  public class FeefoReview
  {
    public int ReviewId { get; set; }

    public int FeedbackId { get; set; }

    public DateTime ReviewDate { get; set; }

    public string ProductCode { get; set; }

    public int Rating { get; set; }

    public string CustomerComment { get; set; }

    public string OrderRefNumber { get; set; }

    public string CustomerEmail { get; set; }

    public string CustomerName { get; set; }

    public string Mode { get; set; }

    public string VendorComment { get; set; }

    public Guid AffiliateId { get; set; }

    public DateTime CreatedOn { get; set; }
  }
}
