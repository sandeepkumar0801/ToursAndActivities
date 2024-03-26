using Isango.Entities.Review;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Util;

namespace Isango.Persistence.Data
{
    public class ReviewData
    {
        /// <summary>
        /// This method returns productive review data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Review GetProductReviewData(IDataReader reader)
        {
            var review = new Review
            {
                Title = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "title"),
                Text = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "reviewcomments"),
                Rating = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "overallrating"),
                UserName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "firstname"),
                Country = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "country"),
                SubmittedDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "reviewedon")
            };

            return review;
        }

        /// <summary>
        /// This method returns review data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Review GetReviewData(IDataReader reader)
        {
            var userReview = new Review();

            if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "reviewcomments")))
                userReview.Text = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "reviewcomments");
            if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Title")))
                userReview.Title = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "title");
            if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ReviewDate")))
                userReview.SubmittedDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "ReviewDate");
            if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "overallRating")))
                userReview.Rating = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "overallRating");

            userReview.UserName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "reviewer_name");
            userReview.Country = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Country");

            if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "reviewid")))
                userReview.Id = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "reviewid");

            userReview.ServiceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceID");
            userReview.ServiceName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceName");

            return userReview;
        }

        /// <summary>
        /// This method returns all productive review data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<RegionReviews> GetAllProductReviewData(IDataReader reader)
        {
            List<RegionReviews> regReviews = null;

            while (reader.Read())
            {
                if (regReviews == null)
                    regReviews = new List<RegionReviews>();
                var regId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "regionid");
                var regName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "regionname");
                var reviewItem = regReviews.FirstOrDefault(p => p.Id == regId);
                if (reviewItem == null)
                {
                    reviewItem = new RegionReviews
                    {
                        Id = regId,
                        Name = regName,
                        Reviews = new List<Review>()
                    };
                    regReviews.Add(reviewItem);
                }

                var review = new Review
                {
                    Id = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "reviewid"),
                    Title = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "title"),
                    ServiceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "serviceid"),
                    ServiceName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "servicename"),
                    ServiceReviewsCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceReviewcount"),
                    Text = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "reviewcomments"),
                    Rating = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Reviewrating"),
                    UserName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ReviewerName"),
                    Country = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "country"),
                    ImageId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "productimageid"),
                    ImageName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "imagefilename"),
                    SubmittedDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "ReviewDate")
                };
                reviewItem.Reviews.Add(review);
            }

            return regReviews;
        }
    }
}