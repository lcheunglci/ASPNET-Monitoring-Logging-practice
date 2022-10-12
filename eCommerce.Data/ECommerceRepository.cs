using eCommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Data
{
    public class ECommerceRepository : IECommerceRepository
    {
        private readonly LocalContext _context;

        public ECommerceRepository(LocalContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsAsync(string category)
        {
            return await _context.Products.Where(p => p.Category == category || category == "all").ToListAsync();
        }

        public Product? GetProductById(int id)
        {
            return _context.Products.Find(id);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public List<Product> GetProducts(string category)
        {
            return _context.Products.Where(p => p.Category == category || category == "all").ToList();
        }
    }
}
