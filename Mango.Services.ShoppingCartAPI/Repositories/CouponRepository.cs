using Mango.Services.ShoppingCartAPI.Models.Dto;
using MangoLibrary;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _client;

        public CouponRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var response = await _client.GetDeserializeHttpResponseContent<ResponseDto>($"/api/coupon/{couponCode}");

            return response.GetResult<CouponDto>() ?? new();
        }
    }
}
