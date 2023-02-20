using Mango.web.Models;
using Mango.web.services.Iservices;
using MangoLibrary;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;

        public CartController(ICartService cartService, IProductService productService, ICouponService couponService)
        {
            _cartService = cartService;
            _productService = productService;
            _couponService = couponService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDto());
        }
        [Authorize]
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var res = await _cartService.RemoveFromCart<ResponseDto>(cartDetailsId, accessToken);

            if(res?.IsSucces == true)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View(); 
        }

        [Authorize]
        public async Task<IActionResult> ApplyCoupon(CartDto cart)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var res = await _cartService.ApplyCoupon<ResponseDto>(cart, accessToken);

            if(res?.IsSucces == true)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> RemoveCoupon(CartDto cart)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var res = await _cartService.RemoveCoupon<ResponseDto>(cart.CartHeader.UserId, accessToken);

            if (res?.IsSucces == true)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDto());
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CartDto cart)
        {
            try
            {
                var accesToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.Checkout<ResponseDto>(cart.CartHeader, accesToken);
                if (!response.IsSucces)
                {
                    TempData["Error"] = response.DisplayMessage;
                }
                return RedirectToAction(nameof(Confirmation));
            }
            catch
            {
                return View(cart);
            }
        }
        public IActionResult Confirmation()
        {
            return View();
        }
        private async Task<CartDto> LoadCartDto()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var res = await _cartService.GetCartByUserId<ResponseDto>(userId, accessToken);

            var card = res.GetResult<CartDto>();

            if(card.CartHeader != null)
            {
                if (!string.IsNullOrEmpty(card.CartHeader.CouponCode))
                {
                    res = await _couponService.GetCoupon<ResponseDto>(card.CartHeader.CouponCode, accessToken);
                    var coupon = res.GetResult<CouponDto>();
                    card.CartHeader.DiscountTotal = coupon.DiscountAmount;
                }
                card.CartHeader.OrderTotal = card.CartDetails.Sum(x => x.Product.Price * x.Count);
                card.CartHeader.OrderTotal -= card.CartHeader.DiscountTotal;
            }

            return card;
        }
    }
}
