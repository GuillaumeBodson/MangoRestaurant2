using Azure;
using AzureMessageBus;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Messages
{
    public class CheckoutHeaderDto: BaseMessage
    {
        public int CartHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
        public double OrderTotal { get; set; }
        public double DiscountTotal { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime PickupDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CartNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryMonthYear { get; set; }
        public int CartTotalItems { get; set; }
        public IEnumerable<CartDetailsDto> CartDetails  { get; set; }

        public async Task CheckCouponAvailability(Func<Task<CouponDto>> getCoupon)
        {
            if (!string.IsNullOrEmpty(CouponCode))
            {
                CouponDto coupon = await getCoupon();
                if (coupon.DiscountAmount != DiscountTotal)
                {
                    throw new Exception("Coupon Price has chnaged please Confirm");
                }
            }
        }

        public async Task CheckProductsPrice(Func<Task<IEnumerable<ProductDto>>> getProducts)
        {
            var products = await getProducts();

            if (CartDetails.Any(x => x.Product.Price != products.First(y => y.ProductId == x.ProductId).Price))
                throw new Exception("Products price has changed, plase confirm");
        }
    }
}
