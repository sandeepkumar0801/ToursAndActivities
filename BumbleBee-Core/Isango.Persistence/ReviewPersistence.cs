using Isango.Entities;
using Isango.Entities.Review;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class ReviewPersistence : PersistenceBase, IReviewPersistence
    {
        private readonly ILogger _log;
        public ReviewPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Method to add user review in db
        /// </summary>
        /// <param name="userReview"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public int AddReview(UserReview userReview, string userAgent)
        {
            var reviewId = 0;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.AddReviewSp))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.UserID, DbType.String, userReview.Email);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Title, DbType.String, userReview.ReviewData.Title);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ReviewComments, DbType.String, userReview.ReviewData.Text);
                    IsangoDataBaseLive.AddInParameter(command, Constant.OverallRating, DbType.Int32, userReview.ReviewValue);
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingReferenceID, DbType.String, userReview.BookingRef);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ServiceId, DbType.Int32, userReview.ActivityId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.StatusId, DbType.Int32, (int)userReview.Status);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ReviewDate, DbType.DateTime, DateTime.Today);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ClientBrowser, DbType.String, userAgent);
                    IsangoDataBaseLive.AddOutParameter(command, Constant.ReviewID, DbType.Int32, reviewId);
                    IsangoDataBaseLive.ExecuteNonQuery(command);

                    reviewId = int.Parse(command.Parameters[Constant.ReviewID].Value.ToString());
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewPersistence",
                    MethodName = "AddReview",
                    Params = $"{SerializeDeSerializeHelper.Serialize(userReview)}, {userAgent}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return reviewId;
        }

        /// <summary>
        /// Method to add user review with image in db
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="file"></param>
        /// <param name="sizeInBytes"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public bool AddReviewImage(int reviewId, string file, int sizeInBytes, string caption)
        {
            int status;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.AddReviewImageSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ReviewID, DbType.Int32, reviewId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Url, DbType.String, file);
                    IsangoDataBaseLive.AddInParameter(command, Constant.MinSize, DbType.Int32, 0);
                    IsangoDataBaseLive.AddInParameter(command, Constant.MaxSize, DbType.Int32, sizeInBytes);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Caption, DbType.String, caption);
                    status = IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewPersistence",
                    MethodName = "AddReviewImage",
                    Params = $"{reviewId}, {file}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return Convert.ToBoolean(status);
        }

        /// <summary>
        /// Method to get product review by service id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalresults"></param>
        /// <returns></returns>
        public Tuple<List<Review>, int> GetProductReviews(int serviceId, int pageSize, int pageNumber)
        {
            List<Review> reviews = null;
            int totalresults;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetProductReviewsSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ActivityId, DbType.Int32, serviceId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.RecsPerPage, DbType.Int32, pageSize);
                    IsangoDataBaseLive.AddInParameter(command, Constant.PageNumber, DbType.Int32, pageNumber);
                    IsangoDataBaseLive.AddOutParameter(command, Constant.TotalRecords, DbType.Int32, 5);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var reviewData = new ReviewData();
                        while (reader.Read())
                        {
                            if (reviews == null)
                                reviews = new List<Review>();

                            reviews.Add(reviewData.GetProductReviewData(reader));
                        }
                        reader.Close();
                        totalresults = Convert.ToInt32(IsangoDataBaseLive.GetParameterValue(command, Constant.TotalRecords));
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewPersistence",
                    MethodName = "GetProductReviews",
                    Params = $"{serviceId}, {pageSize}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            var output = Tuple.Create(reviews, totalresults);
            return output;
        }

        /// <summary>
        /// Method to get product review by affiliate id
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalresults"></param>
        /// <returns></returns>
        public Tuple<List<Review>, int> GetReviews(string affiliateId, int pageSize, int pageNumber)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetReviewsSp))
                {
                    Tuple<List<Review>, int> reviewsResult;

                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId, DbType.String, affiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.RecsPerPage, DbType.Int32, pageSize);
                    IsangoDataBaseLive.AddInParameter(command, Constant.PageNumber, DbType.Int32, pageNumber);
                    int totalresults;
                    IsangoDataBaseLive.AddOutParameter(command, Constant.TotalRecords, DbType.Int32, 5);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        List<Review> reviews = null;
                        var reviewData = new ReviewData();
                        while (reader.Read())
                        {
                            if (reviews == null)
                                reviews = new List<Review>();

                            reviews.Add(reviewData.GetReviewData(reader));
                        }

                        reader.Close();
                        totalresults = Convert.ToInt32(IsangoDataBaseLive.GetParameterValue(command, Constant.TotalRecords));
                        if (totalresults <= 0)
                            reviews = null;

                        reviewsResult = Tuple.Create(reviews, totalresults);
                    }
                    return reviewsResult;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewPersistence",
                    MethodName = "GetReviews",
                    Params = $"{affiliateId}, {pageSize}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to get list of all product review
        /// </summary>
        /// <returns></returns>
        public List<RegionReviews> GetAllProductReviewsData()
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAllProductReviewsDataSp))
                {
                    List<RegionReviews> regReviews;
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var reviewData = new ReviewData();
                        regReviews = reviewData.GetAllProductReviewData(reader);
                    }
                    return regReviews;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewPersistence",
                    MethodName = "GetAllProductReviewsData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}