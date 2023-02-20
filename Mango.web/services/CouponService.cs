using Mango.web.Models;
using Mango.web.Models.Factories;
using Mango.web.services.Iservices;

namespace Mango.web.services
{
    public class CouponService : BaseService, ICouponService
    {
        private readonly IHttpRequestMessageFactory _requestMessageFactory;

        public CouponService(IHttpClientFactory httpClient, IHttpRequestMessageFactory requestMessageFactory, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {
            _requestMessageFactory = requestMessageFactory;
        }

        public async Task<T> GetCoupon<T>(string couponCode, string token = null)
        {
            return await SendAsync<T>(_requestMessageFactory.CreateGet(SD.CouponAPIBase + "/api/coupon/" + couponCode));
        }
    }
}
