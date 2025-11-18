using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreServices.ReviewsServices
{
    public interface IReviews
    {
        Task<ReviewsPageDto> GetByProductAsync(int productId, int page, int pageSize);
        Task<double> GetAverageRatingAsync(int productId);
        Task<ReviewDto> AddAsync(ReviewDto dto);
        Task<IEnumerable<ReviewDto>> GetByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int reviewId, int? userId = null, bool isAdmin = false);
        Task<IEnumerable<ReviewDto>> GetAllAsync(int page = 1, int pageSize = 20);
    }
}


