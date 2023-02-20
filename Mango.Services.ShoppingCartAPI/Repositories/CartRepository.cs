using AutoMapper;
using Mango.Services.ShoppingCartAPI.DBContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CartRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId),
            };

            if(cart.CartHeader != null)
                cart.CartDetails = _dbContext.CartDetails.Where(x => cart.CartHeader.CartHeaderId == x.CartHeaderId).Include(x => x.Product);

            return _mapper.Map<CartDto>(cart);
        }
        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);

            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);

            if(product == null)
            {
                _dbContext.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _dbContext.SaveChangesAsync();
            }

            var cartheaderDb = _dbContext.CartHeaders.AsNoTracking().FirstOrDefault(c => c.UserId == cartDto.CartHeader.UserId);

            if (cartheaderDb == null)
            {
                _dbContext.CartHeaders.Add(cart.CartHeader);
                await _dbContext.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var CartdetailsDb = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(c =>
                c.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                c.CartHeaderId == cartheaderDb.CartHeaderId);

                if(CartdetailsDb == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartheaderDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Count += CartdetailsDb.Count;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().CartDetailsId = CartdetailsDb.CartDetailsId;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = CartdetailsDb.CartHeaderId;
                    _dbContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDto>(cart);
        }
        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                var cartDetails = await _dbContext.CartDetails.FirstOrDefaultAsync(x => x.CartDetailsId == cartDetailsId);

                if (cartDetails == null)
                    return false;


                if ((await _dbContext.CartDetails.CountAsync(x => x.CartHeaderId == cartDetails.CartHeaderId)) == 1)
                {
                    var cartHeader = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);
                    _dbContext.CartHeaders.Remove(cartHeader);
                }

                _dbContext.CartDetails.Remove(cartDetails);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderDb = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);

            if(cartHeaderDb != null)
            {
                _dbContext.CartDetails.RemoveRange(_dbContext.CartDetails.Where(x => x.CartHeaderId == cartHeaderDb.CartHeaderId));
                _dbContext.CartHeaders.Remove(cartHeaderDb);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ApplyCoupon(string userId, string coupnCode)
        {
            var cart = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);

            cart.CouponCode = coupnCode;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cart = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);

            cart.CouponCode = null;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
