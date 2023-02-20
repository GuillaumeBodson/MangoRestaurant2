using Mango.web.Models;

namespace Mango.web.services.Iservices
{
    public interface ICartService
    {
        Task<T> AddToCart<T>(CartDto cartDto, string token = null);
        Task<T> GetCartByUserId<T>(string userId, string token = null);
        Task<T> RemoveFromCart<T>(int cartId, string token = null);
        Task<T> UpdateCart<T>(CartDto cartDto, string token = null);
        Task<CartDto> BuildCartDto(ProductDto product);
        Task<T> ApplyCoupon<T>(CartDto cart, string token = null);
        Task<T> RemoveCoupon<T>(string userId, string token = null);
        Task<T> Checkout<T>(CartHeaderDto cartHeader, string token = null);
    }
}