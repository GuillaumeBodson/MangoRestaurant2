using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.CouponAPI.Repository;
using MangoLibrary;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/coupon")]
    public class CouponAPIController : Controller
    {
        private readonly ICouponRepository _couponRepository;
        private ResponseDto _response;
        public CouponAPIController(ICouponRepository couponRepositoryc)
        {
            _response = new ResponseDto();
            _couponRepository = couponRepositoryc;
        }
        [HttpGet("{couponCode}")]
        public async Task<object> GetDiscountForCode(string couponCode)
        {
            await _response.SetResult(async () => await _couponRepository.GetCouponByCode(couponCode));

            return _response;
        }
    }
}
