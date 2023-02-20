using AutoMapper;
using Mango.Services.CouponAPI.DbContexts;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public CouponRepository(IMapper mapper, ApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var coupon = await _dbContext.Coupons.FirstOrDefaultAsync(x => x.CouponCode == couponCode);

            return _mapper.Map<CouponDto>(coupon);
        }
    }
}
