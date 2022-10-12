using eCommerce.Data.Entities;

namespace eCommerce.Data
{
    public interface IECommerceRepository
    {
        Task<List<Product>> GetProductsAsync(string category);
        Task<Product?> GetProductByIdAsync(int id);

        List<Product> GetProducts(string category);
        Product? GetProductById(int id);
    }
}