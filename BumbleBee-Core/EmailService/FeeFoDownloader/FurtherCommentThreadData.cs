using System;

namespace FeefoDownloader
{
  public class FurtherCommentThreadData
  {
    public int ThreadId { get; set; }

    public int FeedBackId { get; set; }

    public DateTime FeedbackDate { get; set; }

    public string ServiceComment { get; set; }

    public string ProductComment { get; set; }

    public int ServiceRating { get; set; }

    public int ProductRating { get; set; }

    public string VendorComment { get; set; }

    public string Md5code { get; set; }
  }
}
