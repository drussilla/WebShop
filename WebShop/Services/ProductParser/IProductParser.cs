namespace WebShop.Services.ProductParser
{
    public interface IProductParser
    {
        ProductParsingResult Parse(string line);
    }
}