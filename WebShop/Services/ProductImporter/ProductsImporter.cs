using System.IO;
using System.Threading.Tasks;
using WebShop.Data.ProductRepository;
using WebShop.Services.ProductParser;

namespace WebShop.Services.ProductImporter
{
    public class ProductsImporter : IProductsImporter
    {
        private readonly IProductParser _productParser;
        private readonly IProductRepository _productRepository;

        public ProductsImporter(IProductParser productParser, IProductRepository productRepository)
        {
            _productParser = productParser;
            _productRepository = productRepository;
        }

        public const string Header =
            "Key,Artikelcode,colorcode,description,price,discountprice,delivered in,q1,size,color";

        public async Task<ImportResult> ImportAsync(Stream file)
        {
            var result = new ImportResult();
            // track current line number, including empty lines
            ulong lineNumber = 0; 
            using (var stream = new StreamReader(file))
            {
                string line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    
                    // skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    // skip header
                    if (line.Equals(Header))
                    {
                        continue;
                    }

                    result.ProcessedItems++;

                    var parsingResult = _productParser.Parse(line);

                    if (!parsingResult.Successful)
                    {
                        result.SkipLine(lineNumber, line, parsingResult.ErrorMessage);
                        continue;
                    }
                    
                    // save or update
                    await _productRepository.AddOrUpdate(parsingResult.Product);
                    result.ImportedItems++;
                }
            }

            return result;
        }
    }
}