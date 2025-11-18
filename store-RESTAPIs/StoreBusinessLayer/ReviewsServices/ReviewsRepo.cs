using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;

namespace StoreServices.ReviewsServices
{
    public class ReviewsRepo : IReviews
    {
        private readonly AppDbContext _db;
        public ReviewsRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ReviewsPageDto> GetByProductAsync(int productId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _db.ProductReviews.AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    UserName = r.User != null ? $"{r.User.FirstName} {r.User.SecondName}" : "مستخدم مجهول",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToArrayAsync();

            var avg = total == 0 ? 0 : await _db.ProductReviews.Where(r => r.ProductId == productId && r.IsApproved).AverageAsync(r => (double)r.Rating);

            return new ReviewsPageDto
            {
                ProductId = productId,
                Page = page,
                PageSize = pageSize,
                Total = total,
                AverageRating = avg,
                Items = items
            };
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var query = _db.ProductReviews.AsNoTracking().Where(r => r.ProductId == productId && r.IsApproved);
            var count = await query.CountAsync();
            if (count == 0) return 0;
            return await query.AverageAsync(r => (double)r.Rating);
        }

        public async Task<ReviewDto> AddAsync(ReviewDto dto)
        {
            if (dto.UserId == null)
            {
                throw new Exception("يجب تسجيل الدخول لإضافة تقييم.");
            }

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                throw new Exception("قيمة التقييم غير صالحة.");
            }

            dto.Comment = dto.Comment?.Trim() ?? string.Empty;

            var clientId = await _db.Clients
                .Where(c => c.UserId == dto.UserId)
                .Select(c => c.ClientId)
                .FirstOrDefaultAsync();

            if (clientId == 0)
            {
                throw new Exception("لم يتم العثور على بيانات العميل.");
            }

            const byte DeliveredStatusId = 4;
            var hasDeliveredOrder = await _db.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.ProductDetails)
                .AnyAsync(od =>
                    od.Order.ClientId == clientId &&
                    od.Order.OrderStatusId == DeliveredStatusId &&
                    od.ProductDetails.ProductId == dto.ProductId);

            if (!hasDeliveredOrder)
            {
                throw new Exception("يمكنك إضافة تقييم بعد استلام المنتج فقط.");
            }

            var alreadyReviewed = await _db.ProductReviews
                .AnyAsync(r => r.ProductId == dto.ProductId && r.UserId == dto.UserId);
            if (alreadyReviewed)
            {
                throw new Exception("لقد قمت بإضافة تقييم لهذا المنتج من قبل.");
            }

            var entity = new ProductReview
            {
                ProductId = dto.ProductId,
                UserId = dto.UserId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                IsApproved = true
            };
            _db.ProductReviews.Add(entity);
            await _db.SaveChangesAsync();
            dto.Id = entity.Id;
            dto.CreatedAt = entity.CreatedAt;
            return dto;
        }

        public async Task<IEnumerable<ReviewDto>> GetByUserIdAsync(int userId)
        {
            return await _db.ProductReviews.AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    UserName = r.User != null ? $"{r.User.FirstName} {r.User.SecondName}" : "مستخدم مجهول",
                    ProductName = r.Product != null ? r.Product.ProductNameAr : "منتج محذوف",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToArrayAsync();
        }

        public async Task<bool> DeleteAsync(int reviewId, int? userId = null, bool isAdmin = false)
        {
            var review = await _db.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);
            
            if (review == null) return false;
            
            // التحقق من الصلاحيات: المستخدم يمكنه حذف تعليقاته فقط، أو الأدمن يمكنه حذف أي تعليق
            if (!isAdmin && (userId == null || review.UserId != userId))
            {
                return false;
            }
            
            _db.ProductReviews.Remove(review);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllAsync(int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            return await _db.ProductReviews.AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    UserName = r.User != null ? $"{r.User.FirstName} {r.User.SecondName}" : "مستخدم مجهول",
                    ProductName = r.Product != null ? r.Product.ProductNameAr : "منتج محذوف",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToArrayAsync();
        }
    }
}


