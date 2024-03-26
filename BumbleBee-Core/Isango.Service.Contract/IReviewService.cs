using Isango.Entities.Review;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IReviewService
    {
        Task<Tuple<List<Review>, int>> GetProductReviewsAsync(int serviceId, int pageSize, int pageNumber);

        Task<int> AddReviewAsync(UserReview usrReview, string userAgent);

        Task<bool> AddReviewImageAsync(int reviewId, string file, int sizeInBytes, string caption);

        Task<Tuple<List<Review>, int>> GetReviewsAsync(string affiliateId, int pageSize, int pageNumber);

        Task<List<RegionReviews>> GetAllProductReviewsDataAsync();
    }
}