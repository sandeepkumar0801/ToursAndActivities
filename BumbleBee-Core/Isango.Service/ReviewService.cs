using Isango.Entities;
using Isango.Entities.Review;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using Util;

namespace Isango.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewPersistence _reviewPersistence;
        private readonly ILogger _log;

        public ReviewService(IReviewPersistence reviewPersistence, ILogger log)
        {
            _reviewPersistence = reviewPersistence;
            _log = log;
        }

        /// <summary>
        /// Method to add user review in db
        /// </summary>
        /// <param name="usrReview"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public async Task<int> AddReviewAsync(UserReview usrReview, string userAgent)
        {
            try
            {
                return await Task.FromResult(_reviewPersistence.AddReview(usrReview, userAgent));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewService",
                    MethodName = "AddReview",
                    Params = $"{SerializeDeSerializeHelper.Serialize(usrReview)},{userAgent}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to add user review with image in db
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="file"></param>
        /// <param name="sizeInBytes"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public async Task<bool> AddReviewImageAsync(int reviewId, string file, int sizeInBytes, string caption)
        {
            try
            {
                return await Task.FromResult(_reviewPersistence.AddReviewImage(reviewId, file, sizeInBytes, caption));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewService",
                    MethodName = "AddReviewImage",
                    Params = $"{reviewId}{file}{sizeInBytes}{caption}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to get product review by service id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalresults"></param>
        /// <returns></returns>
        public async Task<Tuple<List<Review>, int>> GetProductReviewsAsync(int serviceId, int pageSize, int pageNumber)
        {
            try
            {
                return await Task.FromResult(_reviewPersistence.GetProductReviews(serviceId, pageSize, pageNumber));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewService",
                    MethodName = "GetProductReviews",
                    Params = $"{serviceId}{pageSize}{pageNumber}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to get product review by affiliate id
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalresults"></param>
        /// <returns></returns>
        public async Task<Tuple<List<Review>, int>> GetReviewsAsync(string affiliateId, int pageSize, int pageNumber)
        {
            try
            {
                return await Task.FromResult(_reviewPersistence.GetReviews(affiliateId, pageSize, pageNumber));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewService",
                    MethodName = "GetReviews",
                    Params = $"{affiliateId}{pageSize}{pageNumber}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Method to get list of all product review
        /// </summary>
        /// <returns></returns>
        public async Task<List<RegionReviews>> GetAllProductReviewsDataAsync()
        {
            try
            {
                return await Task.FromResult(_reviewPersistence.GetAllProductReviewsData());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ReviewService",
                    MethodName = "GetAllProductReviewsData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}