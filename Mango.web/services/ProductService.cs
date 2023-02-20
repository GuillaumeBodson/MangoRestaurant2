using Mango.web.Models;
using Mango.web.Models.Factories;
using Mango.web.services.Iservices;

namespace Mango.web.services
{
    public class ProductService : BaseService,IProductService
    {
        private readonly IHttpRequestMessageFactory _requestMessageFactory;

        public ProductService(IHttpClientFactory httpClientFactory, IHttpRequestMessageFactory requestMessageFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor)
        {
            _requestMessageFactory = requestMessageFactory;
        }

        public async Task<T> CreateProductAsync<T>(ProductDto product, string token)
        {
            return await SendAsync<T>(_requestMessageFactory.CreatePost(SD.ProductApiBase + "/api/products", product));
        }

        public async Task<T> DeleteProductAsync<T>(int id, string token)
        {
            return await SendAsync<T>(_requestMessageFactory.CreateDelete(SD.ProductApiBase + "/api/products/" + id));
        }


        public async Task<T> GetAllProductsAsync<T>()
        {
            return await SendAsync<T>(_requestMessageFactory.CreateGet(SD.ProductApiBase + "/api/products"));
        }

        public async Task<T> GetProductByIdAsync<T>(int id, string token)
        {
            return await SendAsync<T>(_requestMessageFactory.CreateGet(SD.ProductApiBase + "/api/products/" + id)); 
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto product, string token)  
        {
            return await SendAsync<T>(_requestMessageFactory.CreatePut(SD.ProductApiBase + "/api/products", product));
        }
    }
}
