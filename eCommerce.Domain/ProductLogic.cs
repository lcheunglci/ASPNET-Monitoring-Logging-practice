using eCommerce.Data;
using eCommerce.Data.Entities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace eCommerce.Domain;

public class ProductLogic : IProductLogic
{
    private readonly ILogger<ProductLogic> _logger;
    private readonly IECommerceRepository _repo;

    public ProductLogic(ILogger<ProductLogic> logger, IECommerceRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public Product? GetProductById(int id)
    {
        _logger.LogDebug("Logic for single product id {id}", id);
        return _repo.GetProductById(id);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _repo.GetProductByIdAsync(id);
    }

    public IEnumerable<Product> GetProductsForCategory(string category)
    {
        return _repo.GetProducts(category);
    }

    public async Task<IEnumerable<Product>> GetProductsForCategoryAsync(string category)
    {
        _logger.LogInformation("Getting products in logic for {category}", category);

        Activity.Current?.AddEvent(new ActivityEvent("Getting products from repository"));
        var result  = await _repo.GetProductsAsync(category);
        Activity.Current?.AddEvent(new ActivityEvent("Retrieved products from repository"));


        return result;
    }
}