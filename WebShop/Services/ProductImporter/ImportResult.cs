using System.Collections.Generic;

namespace WebShop.Services.ProductImporter
{
    public class ImportResult
    {
        private readonly List<SkippedItemDetails> _skippedItems;

        public ImportResult()
        {
            _skippedItems = new List<SkippedItemDetails>();
        }

        public bool Successful => ImportedItems > 0;

        public int ProcessedItems { get; set; }

        public int ImportedItems { get; set; }

        public int SkippedItems => _skippedItems.Count;

        public IReadOnlyList<SkippedItemDetails> SkippedItemsDetails => _skippedItems;

        public void SkipLine(ulong lineNumber, string line, string errorMessage)
        {
            _skippedItems.Add(new SkippedItemDetails
            {
                Description = errorMessage,
                LineNumber = lineNumber,
                OriginalLine = line
            });
    }
    }
}