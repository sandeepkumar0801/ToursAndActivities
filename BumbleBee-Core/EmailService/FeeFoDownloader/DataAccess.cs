using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FeefoDownloader
{
  public class DataAccess
  {
    private const string FeefoThreadReviews_INSERT_BULK_COMMAND = "insert into FurtherCommentThreadDataTemp (FeedBackId, FeedbackDate, ServiceComment, ProductComment, ServiceRating, ProductRating, VendorComment, Md5code) values (@FeedBackId, @FeedbackDate, @ServiceComment, @ProductComment, @ServiceRating, @ProductRating, @VendorComment, @Md5code)";
    private const string FeefoReviews_INSERT_BULK_COMMAND = "INSERT INTO FeefoReviewTemp (FeedbackId ,ReviewDate ,ProductCode ,Rating ,CustomerComment ,OrderRefNumber ,CustomerEmail ,CustomerName ,Mode ,VendorComment ,AffiliateId, CreatedOn) VALUES (@FeedbackId ,@ReviewDate ,@ProductCode ,@Rating ,@CustomerComment ,@OrderRefNumber ,@CustomerEmail ,@CustomerName ,@Mode ,@VendorComment ,@AffiliateId, @CreatedOn)";
    private  readonly string _connectionString;


    public DataAccess(IConfiguration config)
    {
        _connectionString = config.GetSection("ConnectionStrings:IsangoDB").Value;
    }
     public List<Affiliate> GetAffiliates()
     {
         string sql = "SELECT AffiliateID, FeefoKey, CompanyWebsite FROM Affiliates WHERE FeefoKey IS NOT NULL AND FeefoKey != '' AND CompanyWebsite IS NOT NULL AND CompanyWebsite != '' AND AffiliateID IS NOT NULL";
         List<Affiliate> affiliates = new List<Affiliate>();

         using (SqlConnection connection = new SqlConnection(_connectionString))
         {
             connection.Open();

             using (SqlCommand command = new SqlCommand(sql, connection))
             {
                 using (SqlDataReader reader = command.ExecuteReader())
                 {
                     while (reader.Read())
                     {
                         Affiliate affiliate = new Affiliate
                         {
                             AffiliateID = (Guid)reader["AffiliateID"],
                             FeefoKey = reader["FeefoKey"].ToString(),
                             CompanyWebsite = reader["CompanyWebsite"].ToString()
                         };
                         affiliates.Add(affiliate);
                     }
                 }
             }
         }

         return affiliates;
     }



        public bool FinalizeData()
        {
            string sql = "usp_ins_FeefoReview";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }

                return true;
            }
        }



        public  bool FeefoReviews_Insert(List<FEEDBACKLISTFEEDBACK> data, string mode, Guid affiliateId)
        {
            try
            {
                List<FeefoReview> feefoList = GetFeefoList(data, mode, affiliateId);
                if (feefoList == null || !feefoList.Any())
                    return false;

                foreach (FEEDBACKLISTFEEDBACK feedback in data.FindAll(p => p.FURTHERCOMMENTSTHREAD != null && p.FURTHERCOMMENTSTHREAD.Any()))
                {
                    FeefoReviewsThread_Insert(feedback.FURTHERCOMMENTSTHREAD, feedback.FEEDBACKID);
                }

                return InsertFeefoReviews(feefoList);
            }
            catch (Exception ex)
            {
                Helper.SendSupportMail("Error on Feefo Reviews Insert", null, ex.Message);
                return false;
            }
        }


        private  bool FeefoReviewsThread_Insert(List<FEEDBACKLISTFEEDBACKPOST> data, int feedbackId)
        {
            try
            {
                List<FurtherCommentThreadData> threadList =GetThreadList(data, feedbackId);
                return threadList != null && threadList.Any() && InsertFeefoReviewThread(threadList);
            }
            catch (Exception ex)
            {
                Helper.SendSupportMail("Error on Feefo Review Thread Insert", null, ex.Message);
                return false;
            }
        }


        private bool InsertFeefoReviews(List<FeefoReview> feefoList)
        {
            string sql = "INSERT INTO FeefoReviewTemp (FeedbackId ,ReviewDate ,ProductCode ,Rating ,CustomerComment ,OrderRefNumber ,CustomerEmail ,CustomerName ,Mode ,VendorComment ,AffiliateId, CreatedOn) VALUES (@FeedbackId ,@ReviewDate ,@ProductCode ,@Rating ,@CustomerComment ,@OrderRefNumber ,@CustomerEmail ,@CustomerName ,@Mode ,@VendorComment ,@AffiliateId, @CreatedOn)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (FeefoReview review in feefoList)
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FeedbackId", review.FeedbackId);
                        command.Parameters.AddWithValue("@ReviewDate", review.ReviewDate);
                        command.Parameters.AddWithValue("@ProductCode", review.ProductCode);
                        command.Parameters.AddWithValue("@Rating", review.Rating);
                        command.Parameters.AddWithValue("@CustomerComment", review.CustomerComment);
                        command.Parameters.AddWithValue("@OrderRefNumber", review.OrderRefNumber);
                        command.Parameters.AddWithValue("@CustomerEmail", review.CustomerEmail);
                        command.Parameters.AddWithValue("@CustomerName", review.CustomerName);
                        command.Parameters.AddWithValue("@Mode", review.Mode);
                        //command.Parameters.AddWithValue("@VendorComment", review.VendorComment);
                        command.Parameters.AddWithValue("@AffiliateId", review.AffiliateId);
                        command.Parameters.AddWithValue("@CreatedOn", review.CreatedOn);
                        if (review.VendorComment == null)
                        {
                            command.Parameters.AddWithValue("@VendorComment", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@VendorComment", review.VendorComment);
                        }


                        command.ExecuteNonQuery();
                    }
                }
            }

            return true;
        }


        private  bool InsertFeefoReviewThread(List<FurtherCommentThreadData> feefoThreadList)
        {
            string sql = "insert into FurtherCommentThreadDataTemp (FeedBackId, FeedbackDate, ServiceComment, ProductComment, ServiceRating, ProductRating, VendorComment, Md5code) values (@FeedBackId, @FeedbackDate, @ServiceComment, @ProductComment, @ServiceRating, @ProductRating, @VendorComment, @Md5code)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (FurtherCommentThreadData threadData in feefoThreadList)
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FeedBackId", threadData.FeedBackId);
                        command.Parameters.AddWithValue("@FeedbackDate", threadData.FeedbackDate);
                        command.Parameters.AddWithValue("@ServiceComment", threadData.ServiceComment);
                        command.Parameters.AddWithValue("@ProductComment", threadData.ProductComment);
                        command.Parameters.AddWithValue("@ServiceRating", threadData.ServiceRating);
                        command.Parameters.AddWithValue("@ProductRating", threadData.ProductRating);
                        command.Parameters.AddWithValue("@VendorComment", threadData.VendorComment);
                        command.Parameters.AddWithValue("@Md5code", threadData.Md5code);

                        command.ExecuteNonQuery();
                    }
                }
            }

            return true;
        }


        private static List<FurtherCommentThreadData> GetThreadList(
      List<FEEDBACKLISTFEEDBACKPOST> data,
      int feedbackId)
    {
      return data.Select<FEEDBACKLISTFEEDBACKPOST, FurtherCommentThreadData>((System.Func<FEEDBACKLISTFEEDBACKPOST, FurtherCommentThreadData>) (p => new FurtherCommentThreadData()
      {
        FeedbackDate = GetDate(p.DATE),
        FeedBackId = feedbackId,
        ProductRating = GetRatings(p.PRODUCTRATING),
        ServiceRating = GetRatings(p.SERVICERATING),
        VendorComment = p.VENDORCOMMENT,
        ProductComment = GetComments(p.CUSTOMERCOMMENT, false),
        ServiceComment = GetComments(p.CUSTOMERCOMMENT, true),
        Md5code = GetMd5Hash(p.DATE, p.VENDORCOMMENT, p.CUSTOMERCOMMENT)
      })).ToList<FurtherCommentThreadData>();
    }

    private static string GetComments(string comment, bool isService)
    {
      if (string.IsNullOrWhiteSpace(comment))
        return string.Empty;
      if (comment.ToLower().IndexOf("<br/>product :") == -1 & isService)
        return comment;
      string[] strArray = comment.Split(new string[1]
      {
        "<br/>product :"
      }, StringSplitOptions.RemoveEmptyEntries);
      if (isService)
        return strArray[0];
      return strArray.Length <= 1 ? string.Empty : strArray[1];
    }

        public static string GetMd5Hash(string date, string vComment, string cComment)
        {
            string str = string.Format("{0}_{1}_{2}", string.IsNullOrWhiteSpace(date) ? (object)"" : (object)date, string.IsNullOrWhiteSpace(vComment) ? (object)"" : (object)vComment, string.IsNullOrWhiteSpace(cComment) ? (object)"" : (object)cComment);
            byte[] buffer = Encoding.ASCII.GetBytes(str);

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }


    private static DateTime GetDate(string date) => Convert.ToDateTime(date.Replace("T", " "));

    private static int GetRatings(string rating)
    {
      if (string.IsNullOrWhiteSpace(rating))
        return 0;
      switch (rating.Trim())
      {
        case "--":
          return 1;
        case "-":
          return 2;
        case "+":
          return 4;
        case "++":
          return 5;
        default:
          return 0;
      }
    }

    private static List<FeefoReview> GetFeefoList(
      List<FEEDBACKLISTFEEDBACK> data,
      string mode,
      Guid affiliateId)
    {
      return data.Select<FEEDBACKLISTFEEDBACK, FeefoReview>((System.Func<FEEDBACKLISTFEEDBACK, FeefoReview>) (p => new FeefoReview()
      {
        AffiliateId = affiliateId,
        CreatedOn = DateTime.Now,
        CustomerComment = p.CUSTOMERCOMMENT,
        CustomerEmail = p.CUSTOMEREMAIL,
        CustomerName = p.CUSTOMERNAME,
        FeedbackId = p.FEEDBACKID,
        Mode = mode,
        OrderRefNumber = p.ORDERREF,
        ProductCode = p.PRODUCTCODE,
        Rating = p.HREVIEWRATING,
        ReviewDate = p.HREVIEWDATE,
        VendorComment = p.VENDORCOMMENT
      })).ToList<FeefoReview>();
    }
  }
}
