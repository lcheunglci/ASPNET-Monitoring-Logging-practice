using eCommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace eCommerce.Data
{
    public class ECommerceRepository : IECommerceRepository
    {
        private readonly LocalContext _context;
        private readonly ILogger<ECommerceRepository> _logger;
        private readonly ILogger _factoryLogger;

        public ECommerceRepository(LocalContext context, ILogger<ECommerceRepository> logger, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = logger;
            _factoryLogger = loggerFactory.CreateLogger("DataAccessLayer");
        }

        public async Task<List<Product>> GetProductsAsync(string category)
        {
            _logger.LogInformation("Getting products in repository for {category}", category);
            return await _context.Products.Where(p => p.Category == category || category == "all").ToListAsync();
        }

        public Product? GetProductById(int id)
        {
            var timer = new Stopwatch();
            timer.Start();
            var product = _context.Products.Find(id);
            timer.Stop();

            _logger.LogDebug("Querying products for {id} finished in {milliseconds} milliseconds",
                id,
                timer.ElapsedMilliseconds);

            _factoryLogger.LogInformation("(F) Querying products for {id} finished in {ticks} ticks",
                id,
                timer.ElapsedTicks);

            return product;
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
