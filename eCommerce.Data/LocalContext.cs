using eCommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Data
{
    public class LocalContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;

        public string DbPath { get; set; }

        public LocalContext()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DbPath = Path.Join(path, "ecommerce-logging.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        public void MigrateAndCreateData()
        {
            Database.Migrate();

            if (Products.Any()) return;

            Products.Add(new Product
            {
                Name = "Bear",
                Category = "sticker",
                Price = 6.99,
                Description = "Cheerful brown bear sticker to add to your collection.",
                ImgUrl = "/images/bear.png"
            });
            Products.Add(new Product
            {
                Name = "Dog",
                Category = "sticker",
                Price = 4.99,
                Description =
                    "Friendly husky dog sticker to add to your collection of dog stickers.",
                ImgUrl = "/images/dog.png"
            });
            Products.Add(new Product
            {
                Name = "Penguin",
                Category = "sticker",
                Price = 6.99,
                Description =
                    "Playful penguin sticker to add to your collection of article birds.",
                ImgUrl = "/images/penguin.png"
            });
            Products.Add(new Product
            {
                Name = "Sloth",
                Category = "sticker",
                Price = 7.99,
                Description =
                    "The sleepy slow sloth sticker to add to your collection.",
                ImgUrl = "/images/sloth.png"
            });
            Products.Add(new Product
            {
                Name = "Whale",
                Category = "sticker",
                Price = 99.99,
                Description =
                    "To show off that you are whale lover and a whale spender, this sticker is for you.",
                ImgUrl = "/images/whale.png"
            });

            SaveChanges();
        }
    }
}
