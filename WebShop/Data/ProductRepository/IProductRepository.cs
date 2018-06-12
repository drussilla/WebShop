using System.Threading.Tasks;

namespace WebShop.Data.ProductRepository
{
    public interface IProductRepository
    {
        Task AddOrUpdate(Product product);
    }
}