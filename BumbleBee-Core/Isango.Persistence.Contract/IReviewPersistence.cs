using Isango.Entities.Review;
using System;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IReviewPersistence
    {
        Tuple<List<Review>, int> GetProductReviews(int serviceId, int pageSize, int pageNumber);

        int AddReview(UserReview userReview, string userAgent);

        bool AddReviewImage(int reviewId, string file, int sizeInBytes, string caption);

        Tuple<List<Review>, int> GetReviews(string affiliateId, int pageSize, int pageNumber);

        List<RegionReviews> GetAllProductReviewsData();
    }
}