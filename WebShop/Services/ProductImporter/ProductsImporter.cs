using System.IO;
using System.Threading.Tasks;
using WebShop.Services.ProductParser;

namespace WebShop.Services.ProductImporter
{
    public class ProductsImporter : IProductsImporter
    {
        private readonly IProductParser _productParser;

        public ProductsImporter(IProductParser productParser)
        {
            _productParser = productParser;
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
                    result.ImportedItems++;
                }
            }

            return result;
        }
    }
}