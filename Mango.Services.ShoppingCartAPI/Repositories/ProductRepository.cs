using Mango.Services.ShoppingCartAPI.Models.Dto;
using MangoLibrary;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly HttpClient _client;

        public ProductRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts(IEnumerable<int> ids)
        {
            var response = await _client.GetDeserializeHttpResponseContent<ResponseDto>($"api/products/multiple?ids={string.Join("&ids=",ids)}");

            return response.GetResult<IEnumerable<ProductDto>>();
        }
    }
}
