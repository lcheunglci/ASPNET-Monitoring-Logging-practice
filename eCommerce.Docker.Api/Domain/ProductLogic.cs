using eCommerce.Docker.Api.ApiModels;
using eCommerce.Docker.Api.Interfaces;

namespace eCommerce.Docker.Api.Domain
{
    public class ProductLogic : IProductLogic
    {
        private readonly ILogger<ProductLogic> _logger;
        private readonly List<string> _validCategories = new List<string>
        {
            "all", "stickers", "mousepad", "tshirts", "misc"
        };

        public ProductLogic(ILogger<ProductLogic> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Product> GetProductsForCategory(string category)
        {
            _logger.LogInformation("Starting logic to get products", category);

            if (!_validCategories.Any(c => string.Equals(category, c, StringComparison.InvariantCultureIgnoreCase)))
            {
                // invalid category -- bad request
                throw new ApplicationException($"Unrecognized category: {category}.  " +
                         $"Valid categories are: [{string.Join(",", _validCategories)}]");
            }

            if (string.Equals(category, "mousepads", StringComparison.InvariantCultureIgnoreCase))
            {
                // simulate database error or real technical error like not implemented exception
                throw new Exception("Not implemented! No mousepads have been defined in 'database' yet!!!!");
            }

            return GetAllProducts().Where(a =>
                string.Equals("all", category, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(category, a.Category, StringComparison.InvariantCultureIgnoreCase));
        }

        private static IEnumerable<Product> GetAllProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Cats", Category = "stickers", Price = 3.99,
                    Description = "Meow says the cute little kitty cat!" },
                new Product { Id = 2, Name = "Dog", Category = "stickers", Price = 4.99,
                    Description = "Woof says the brave little puppy dog." },
                new Product { Id = 3, Name = "Cow", Category = "stickers", Price = 4.59,
                    Description = "Moo says the cow eating hay on the farm."},
                new Product { Id = 4, Name = "Duck", Category = "stickers", Price = 1.99,
                    Description = "Quack says the duck swimming in the pond." },
            };
        }
    }
}
