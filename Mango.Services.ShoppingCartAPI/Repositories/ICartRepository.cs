using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public interface ICartRepository
    {
        Task<bool> ClearCart(string userId);
        Task<CartDto> CreateUpdateCart(CartDto cartDto);
        Task<CartDto> GetCartByUserId(string userId);
        Task<bool> RemoveFromCart(int CartDetailsId);
        Task<bool> ApplyCoupon(string userId, string coupnCode);
        Task<bool> RemoveCoupon(string coupnCode);
    }
}