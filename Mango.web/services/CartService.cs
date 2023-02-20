using Mango.web.Models;
using Mango.web.Models.Factories;
using Mango.web.services.Iservices;
using MangoLibrary;

namespace Mango.web.services
{
    public class CartService : BaseService, ICartService
    {
        private readonly IHttpRequestMessageFactory _requestMessageFactory;
        private readonly IProductService _productService;

        public CartService(IHttpClientFactory httpClient, IHttpRequestMessageFactory requestMessageFactory, IProductService productService, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {
            _requestMessageFactory = requestMessageFactory;
            _productService = productService;
        }

        public async Task<T> GetCartByUserId<T>(string userId, string token = null)
        {
            return await SendAsync<T>(_requestMessageFactory.CreateGet(SD.ShoppingCartAPI + "/api/cart/GetCart/" + userId));
        }
        public async Task<T> AddToCart<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(_requestMessageFactory.CreatePost(SD.ShoppingCartAPI + "/api/cart/AddCart/", cartDto));
        }
        public async Task<T> UpdateCart<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(_requestMessageFactory.CreatePost(SD.ShoppingCartAPI + "/api/cart/UpdateCart/", cartDto));
        }
        public async Task<T> RemoveFromCart<T>(int cartId, string token = null)
        {
            return await SendAsync<T>(_requestMessageFactory.CreatePost(SD.ShoppingCartAPI + "/api/cart/RemoveCart/", cartId));
        }
        public async Task<CartDto> BuildCartDto(ProductDto product)
        {
            var resp = await _productService.GetProductByIdAsync<ResponseDto>(product.ProductId, "");

            return new CartDto()
            {
                CartHeader = new()
                {
                    UserId = _httpContextAccessor.HttpContext.User.FindFirst("sub").Value
                },
                CartDetails = new List<CartDetailsDto>
                {
                    new()
                    {
                        Count = product.Count,
                        ProductId = product.ProductId,
                        Product = resp.GetResult<ProductDto>(),
                    }
                }
            };
        }

        public async Task<T> ApplyCoupon<T>(CartDto cart, string token = null)
        {
            return await SendAsync<T>(_requestMessageFactory.CreatePost(SD.ShoppingCartAPI + "/api/cart/ApplyCoupon", cart));
        }

        public async Task<T> RemoveCoupon<T>(string userId, string token = null)
        {
            return await SendAsync<T>(_requestMessageFactory.CreatePost(SD.ShoppingCartAPI + "/api/cart/RemoveCoupon", userId));
        }
        public async Task<T> Checkout<T>(CartHeaderDto cartHeader, string  token = null)
        {
            return await SendAsync<T>(_requestMessageFactory.CreatePost(SD.ShoppingCartAPI + "/api/cart/Checkout", cartHeader));
        }
    }
}
