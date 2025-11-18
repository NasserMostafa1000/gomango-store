using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreServices.ReviewsServices
{
    public interface IReviewReplies
    {
        Task<IEnumerable<ReviewReplyDto>> GetByReviewIdAsync(int reviewId);
        Task<ReviewReplyDto> AddAsync(ReviewReplyDto dto);
        Task<bool> DeleteAsync(int replyId, int? userId = null, bool isAdmin = false);
    }
}


