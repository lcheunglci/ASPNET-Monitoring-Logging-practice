using eCommerce.Docker.Api.ApiModels;
using eCommerce.Docker.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace eCommerce.Docker.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductLogic _productLogic;
        
        public ProductsController(IProductLogic productLogic)
        {
            _productLogic = productLogic;
        }

        [HttpGet]
        public IEnumerable<Product> GetProducts(string category = "all")
        {
            //Log.Information("Starting controller action GetProducts for {category}", category);

            // makes it easier to search and review in Seq
            Log.ForContext("Category", category)
                .Information("Starting controller action GetProducts");

            return _productLogic.GetProductsForCategory(category);
        }

    }
}
