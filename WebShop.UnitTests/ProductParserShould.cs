using WebShop.Services.ProductParser;
using Xunit;

namespace WebShop.UnitTests
{
    public class ProductParserShould
    {
        [Theory]
        [InlineData(",,,,,,,,,")] // key is empty
        [InlineData(",,,,,,,,,,,,,,")] // too many delimiters
        [InlineData("123,,,,,qwe,,,,")] // discount price is not a number
        [InlineData("123,,,,,,,,,")] // price is empty
        [InlineData("123,,,,123,123,,,,")] // size is empty
        [InlineData("123,,,,123,123,,,asd,")] // size is NaN
        public void ReturnError_IfFormatIsWrong(string input)
        {
            var target = new ProductParser();

            var actual = target.Parse(input);

            Assert.False(actual.Successful);
            Assert.NotNull(actual.ErrorMessage);
            Assert.Null(actual.Product);
        }

        [Fact]
        public void ReturnProduct_IfFormatIsCorrect()
        {
            var target = new ProductParser();

            var actual = target.Parse("1,2,3,4,5,6,7,8,9,10");

            Assert.True(actual.Successful);
            Assert.Null(actual.ErrorMessage);
            Assert.NotNull(actual.Product);

            // format: Key,Artikelcode,colorcode,description,price,discountprice,delivered in,q1,size,color
            Assert.Equal("1", actual.Product.Key);
            Assert.Equal("2", actual.Product.ArticleCode);
            Assert.Equal("3", actual.Product.ColorCode);
            Assert.Equal("4", actual.Product.Description);
            Assert.Equal(5, actual.Product.Price);
            Assert.Equal(6, actual.Product.DiscountPrice);
            Assert.Equal("7", actual.Product.DeliveredIn);
            Assert.Equal("8", actual.Product.Q1);
            Assert.Equal((uint)9, actual.Product.Size);
            Assert.Equal("10", actual.Product.Color);
        }
    }
}
