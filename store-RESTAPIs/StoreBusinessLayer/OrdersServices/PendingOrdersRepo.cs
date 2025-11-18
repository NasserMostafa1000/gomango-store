using Microsoft.EntityFrameworkCore;
using System.Linq;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;

namespace StoreBusinessLayer.Orders
{
    public interface IPendingOrderStore
    {
        Task<PendingOrder> CreateAsync(PendingOrder pendingOrder);
        Task<PendingOrder?> GetBySessionIdAsync(string sessionId);
        Task<IReadOnlyList<PendingOrder>> GetAllAsync(bool includeCompleted = false);
        Task UpdateAsync(PendingOrder pendingOrder);
        Task DeleteAsync(int pendingOrderId);
    }

    public class PendingOrderStore : IPendingOrderStore
    {
        private readonly AppDbContext _context;

        public PendingOrderStore(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PendingOrder> CreateAsync(PendingOrder pendingOrder)
        {
            await _context.PendingOrders.AddAsync(pendingOrder);
            await _context.SaveChangesAsync();
            return pendingOrder;
        }

        public async Task<PendingOrder?> GetBySessionIdAsync(string sessionId)
        {
            return await _context.PendingOrders.FirstOrDefaultAsync(p => p.StripeSessionId == sessionId);
        }

        public async Task<IReadOnlyList<PendingOrder>> GetAllAsync(bool includeCompleted = false)
        {
            var query = _context.PendingOrders.AsQueryable();
            if (!includeCompleted)
            {
                query = query.Where(p => !p.IsCompleted);
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(PendingOrder pendingOrder)
        {
            _context.PendingOrders.Update(pendingOrder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int pendingOrderId)
        {
            var pendingOrder = await _context.PendingOrders.FindAsync(pendingOrderId);
            if (pendingOrder != null)
            {
                _context.PendingOrders.Remove(pendingOrder);
                await _context.SaveChangesAsync();
            }
        }
    }
}

