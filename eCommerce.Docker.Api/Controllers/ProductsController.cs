using eCommerce.Docker.Api.ApiModels;
using eCommerce.Docker.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Docker.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductLogic _productLogic;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger, IProductLogic productLogic)
        {
            _productLogic = productLogic;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Product> GetProducts(string category = "all")
        {
            _logger.LogInformation("Starting controller action GetProducts for {category}", category);
            
            return _productLogic.GetProductsForCategory(category);
        }

    }
}
