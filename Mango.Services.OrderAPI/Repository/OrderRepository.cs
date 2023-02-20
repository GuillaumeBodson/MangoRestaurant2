using AutoMapper;
using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            try
            {
                await using var db = new ApplicationDbContext(_dbContext);
                db.OrderHeaders.Add(orderHeader);
                await db.SaveChangesAsync();
                return true;
            }
            catch { }
            return false;
        }
        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool isPaid)
        {
            await using var db = new ApplicationDbContext(_dbContext);
            var orderHeader = await db.OrderHeaders.FirstOrDefaultAsync(x => x.OrderHeaderId == orderHeaderId);
            
            if(orderHeader == null)
                return;

            orderHeader.PaymenStatus = isPaid;
            await db.SaveChangesAsync();
        }
    }
}
