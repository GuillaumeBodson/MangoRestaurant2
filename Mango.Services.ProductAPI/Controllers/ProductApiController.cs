using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository;
using MangoLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    public class ProductApiController : ControllerBase
    {
        protected ResponseDto _response;
        private IProductRepository _productRepository;

        public ProductApiController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            await _response.SetResult(async () => await _productRepository.GetProducts());

            return _response;
        }

        [HttpGet("multiple")]
        public async Task<object> Get([FromQuery]IEnumerable<int> ids)
        {
            await _response.SetResult(async () => (await _productRepository.GetProducts()).Where(x => ids.Contains(x.ProductId)));

            return _response;
        }

        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            await _response.SetResult(async () => await _productRepository.GetProductById(id));

            return _response;
        }
        [Authorize]
        [HttpPost]
        public async Task<object> UpdateCreate([FromBody] ProductDto productdto)
        {
            await _response.SetResult(async () => await _productRepository.CreateUpdateProduct(productdto));
            
            return _response;
        }
        [Authorize]
        [HttpPut]
        public async Task<object> Put([FromBody] ProductDto productdto)
        {
            await _response.SetResult(async () => await _productRepository.CreateUpdateProduct(productdto));

            return _response;
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<object> Delete(int id)
        {
            await _response.SetResult(async () => await _productRepository.DeleteProduct(id));

            return _response;
        }
    }
}
