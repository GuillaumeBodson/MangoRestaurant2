using Mango.web.Models;
using Mango.web.services.Iservices;
using MangoLibrary;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Mango.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetAllProductsAsync<ResponseDto>();

            List<ProductDto> products = response?.GetResult<List<ProductDto>>();

            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, "");

            ProductDto product = response?.GetResult<ProductDto>();

            return View(product);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ProductDto productDto)
        {
            var cart = await _cartService.BuildCartDto(productDto);

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var res = await _cartService.AddToCart<ResponseDto>(cart, accessToken);

            if (res?.IsSucces == true)
                return RedirectToAction(nameof(Index));

            return View(productDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}