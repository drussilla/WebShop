namespace WebShop.Services.ProductImporter
{
    public class SkippedItemDetails
    {
        public ulong LineNumber { get; set; }
        public string OriginalLine { get; set; }
        public string Description { get; set; }
    }
}