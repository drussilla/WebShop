using WebShop.Data;

namespace WebShop.Services.ProductParser
{
    public class ProductParsingResult
    {
        private ProductParsingResult() { }

        public bool Successful => Product != null;

        public Product Product { get; set; }

        public string ErrorMessage { get; set; }
        
        // Factory method to simplify successful result creation
        public static ProductParsingResult Ok(Product product) => new ProductParsingResult
        {
            Product = product
        };

        // Factory method to simplify error result creation
        public static ProductParsingResult Error(string error) => new ProductParsingResult
        {
            ErrorMessage = error
        };
    }
}