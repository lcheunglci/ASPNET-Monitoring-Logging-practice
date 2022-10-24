using eCommerce.Docker.Api.ApiModels;

namespace eCommerce.Docker.Api.Interfaces
{
    public interface IProductLogic
    {
        IEnumerable<Product> GetProductsForCategory(string category);
    }
}
