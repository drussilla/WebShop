using System.Threading.Tasks;

namespace WebShop.Data.ProductRepository
{
    public interface IProductRepository
    {
        /// <summary>
        /// Adds the or updates product item in the memory. Due to performance limitations its better to Save context to the DB once per request
        /// </summary>
        /// <param name="product">The product.</param>
        Task AddOrUpdate(Product product);

        Task Save();
    }
}