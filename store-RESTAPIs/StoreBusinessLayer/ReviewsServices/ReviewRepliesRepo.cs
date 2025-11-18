using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;

namespace StoreServices.ReviewsServices
{
    public class ReviewRepliesRepo : IReviewReplies
    {
        private readonly AppDbContext _db;
        
        public ReviewRepliesRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ReviewReplyDto>> GetByReviewIdAsync(int reviewId)
        {
            return await _db.ReviewReplies.AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.ReviewId == reviewId && r.IsApproved)
                .OrderBy(r => r.CreatedAt)
                .Select(r => new ReviewReplyDto
                {
                    Id = r.Id,
                    ReviewId = r.ReviewId,
                    UserId = r.UserId,
                    UserName = r.User != null ? $"{r.User.FirstName} {r.User.SecondName}" : "مستخدم مجهول",
                    Reply = r.Reply,
                    CreatedAt = r.CreatedAt
                })
                .ToArrayAsync();
        }

        public async Task<ReviewReplyDto> AddAsync(ReviewReplyDto dto)
        {
            var entity = new ReviewReply
            {
                ReviewId = dto.ReviewId,
                UserId = dto.UserId,
                Reply = dto.Reply,
                IsApproved = true
            };
            
            _db.ReviewReplies.Add(entity);
            await _db.SaveChangesAsync();
            
            // جلب البيانات الكاملة بعد الحفظ
            var saved = await _db.ReviewReplies
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == entity.Id);
            
            if (saved != null)
            {
                dto.Id = saved.Id;
                dto.CreatedAt = saved.CreatedAt;
                dto.UserName = saved.User != null ? $"{saved.User.FirstName} {saved.User.SecondName}" : "مستخدم مجهول";
            }
            
            return dto;
        }

        public async Task<bool> DeleteAsync(int replyId, int? userId = null, bool isAdmin = false)
        {
            var reply = await _db.ReviewReplies
                .FirstOrDefaultAsync(r => r.Id == replyId);
            
            if (reply == null) return false;
            
            // التحقق من الصلاحيات: المستخدم يمكنه حذف ردوده فقط، أو الأدمن يمكنه حذف أي رد
            if (!isAdmin && (userId == null || reply.UserId != userId))
            {
                return false;
            }
            
            _db.ReviewReplies.Remove(reply);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}


