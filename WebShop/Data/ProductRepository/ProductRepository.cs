using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebShop.Data.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly WebShopContext _context;

        public ProductRepository(WebShopContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdate(Product product)
        {
            // it's not the most optimial way to add or update the entity, 
            // one of the way to optimize this is to use Key as primary key and 
            // execute INSERT OR UPDATE statement (but still need to take concurency in to account)
            var existingItem = await _context.Products.FirstOrDefaultAsync(x => x.Key.Equals(product.Key));
            if (existingItem != null)
            {
                existingItem.ArticleCode = product.ArticleCode;
                existingItem.Color = product.Color;
                existingItem.ColorCode = product.ColorCode;
                existingItem.DeliveredIn = product.DeliveredIn;
                existingItem.Description = product.Description;
                existingItem.DiscountPrice = product.DiscountPrice;
                existingItem.Price = product.Price;
                existingItem.Q1 = product.Q1;
                existingItem.Size = product.Size;
            }
            else
            {
                // generate Id on a client side in case we need it before it is saved in DB (useful for distributed systems).
                product.Id = Guid.NewGuid();
                await _context.Products.AddAsync(product);
            }
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}