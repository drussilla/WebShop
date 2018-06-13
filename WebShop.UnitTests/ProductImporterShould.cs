using System.IO;
using System.Text;
using System.Threading.Tasks;
using Moq;
using WebShop.Data;
using WebShop.Data.ProductRepository;
using WebShop.Services.ProductImporter;
using WebShop.Services.ProductParser;
using Xunit;

namespace WebShop.UnitTests
{
    public class ProductImporterShould
    {
        [Fact]
        public async Task ReturnImportResultForEachLine()
        {
            // arrange
            var product = new Product
            {
                Key = "1",
                ArticleCode = "2",
                ColorCode = "3",
                Description = "4",
                Price = 5,
                DiscountPrice = 6,
                DeliveredIn = "7",
                Q1 = "8",
                Size = 9,
                Color = "10"
            };

            var parser = new Mock<IProductParser>();
            parser.Setup(x => x.Parse("1,2,3,4,5,6,7,8,9,10")).Returns(ProductParsingResult.Ok(product));
            parser.Setup(x => x.Parse(",,,,,")).Returns(ProductParsingResult.Error("Error"));

            var repository = new Mock<IProductRepository>();

            var target = new ProductsImporter(parser.Object, repository.Object);

            ImportResult result;
            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("1,2,3,4,5,6,7,8,9,10\n,,,,,")))
            {
                result = await target.ImportAsync(stream);
            }

            repository.Verify(x => x.AddOrUpdate(product), Times.Once);
            repository.Verify(x => x.Save(), Times.Once);
            
            Assert.Equal(2, result.ProcessedItems);
            Assert.Equal(1, result.ImportedItems);
            Assert.Equal(1, result.SkippedItems);
            Assert.Single(result.SkippedItemsDetails);
            Assert.Equal((ulong)2, result.SkippedItemsDetails[0].LineNumber);
            Assert.Equal("Error", result.SkippedItemsDetails[0].Description);
            Assert.Equal(",,,,,", result.SkippedItemsDetails[0].OriginalLine);
        }
    }
}