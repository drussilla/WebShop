using System;

namespace WebShop.Data
{
    public class Product
    {
        // Separate internal field for id is used in case Key field changes it's format in the future
        // so all existing foreign keys will still work without any problem
        public Guid Id { get; set; }

        // We will still use Key property to identify product, so if we re import cvs with updated prices
        // they will be updated and not created again
        public string Key { get; set; }
        public string ArticleCode { get; set; }
        public string ColorCode { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double? DiscountPrice { get; set; }
        public TimeSpan DeliveredRangeFrom { get; set; }
        public TimeSpan DeliveredRangeTo { get; set; }
        public string Q1 { get; set; }

        // I assume that size should be an integer, last row in the example is corrupted and user will see warning message
        public uint Size { get; set; }
        public string Color { get; set; }
    }
}