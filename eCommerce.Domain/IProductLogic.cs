using eCommerce.Domain.Models;

namespace eCommerce.Domain;

public interface IProductLogic
{
    Task<IEnumerable<ProductModel>> GetProductsForCategory(string category);
}