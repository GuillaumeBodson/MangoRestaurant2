using AzureMessageBus;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.RabbitMQSender;
using Mango.Services.ShoppingCartAPI.Repositories;
using MangoLibrary;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartAPIController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IMessageBus _messageBus;
        private readonly IProductRepository _productRepository;
        private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;
        private ResponseDto _response;

        public CartAPIController(ICartRepository cartRepository,
                                 ICouponRepository couponRepository,
                                 IMessageBus messageBus,
                                 IProductRepository productRepository,
                                 IRabbitMQCartMessageSender rabbitMQCartMessageSender)
        {
            _cartRepository = cartRepository;
            _couponRepository = couponRepository;
            _messageBus = messageBus;
            _productRepository = productRepository;
            _rabbitMQCartMessageSender = rabbitMQCartMessageSender;
            _response = new ResponseDto();
        }
        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            await _response.SetResult(async () => await _cartRepository.GetCartByUserId(userId));
            return _response;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            await _response.SetResult(async () => await _cartRepository.CreateUpdateCart(cartDto));
            return _response;
        }
        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            await _response.SetResult(async () => await _cartRepository.CreateUpdateCart(cartDto));
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemmoveCart([FromBody]int cartId)
        {
            await _response.SetResult(async () => await _cartRepository.RemoveFromCart(cartId));
            return _response;
        }
        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto Cart)
        {
            await _response.SetResult(async () => await _cartRepository.ApplyCoupon(Cart.CartHeader.UserId, Cart.CartHeader.CouponCode));

            return _response;
        }
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            await _response.SetResult(async () => await _cartRepository.RemoveCoupon(userId));

            return _response;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout([FromBody] CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserId(checkoutHeader.UserId)
                    ?? throw new ArgumentNullException($"No cart registered for the user {checkoutHeader.UserId}");

                await checkoutHeader.CheckCouponAvailability(async () => await _couponRepository.GetCoupon(checkoutHeader.CouponCode));

                checkoutHeader.CartDetails = cart.CartDetails;

                await checkoutHeader.CheckProductsPrice(async () => await _productRepository.GetProducts(checkoutHeader.CartDetails.Select(x => x.ProductId)));

                //await _messageBus.PublishMessage(checkoutHeader, "checkoutmessagetopic");

                //set up message bus to clear shopping cart only when the payment prcocess is a success

                _rabbitMQCartMessageSender.Send(checkoutHeader, "checkoutmessagetopic");
                await _cartRepository.ClearCart(checkoutHeader.UserId);
            }
            catch (Exception ex)
            {
                _response.SetFailure(ex);
            }

            return _response;
        }
    }
}
