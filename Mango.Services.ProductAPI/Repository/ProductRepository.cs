using AutoMapper;
using AutoMapper.QueryableExtensions;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext applicationDb, IMapper mapper)
        {
            _db = applicationDb;
            _mapper = mapper;
        }
        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            if(product.ProductId > 0)
                _db.Products.Update(product);
            else
                _db.Products.Add(product);

            await _db.SaveChangesAsync();

            return await GetProductById(product.ProductId);
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == id);
                _db.Remove(product);
                await _db.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<ProductDto> GetProductById(int id)
        {
            return await _db.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.ProductId == id);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            return await _db.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}
