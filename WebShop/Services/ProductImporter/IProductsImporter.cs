using System.IO;
using System.Threading.Tasks;

namespace WebShop.Services.ProductImporter
{
    public interface IProductsImporter
    {
        Task<ImportResult> ImportAsync(Stream file);
    }
}