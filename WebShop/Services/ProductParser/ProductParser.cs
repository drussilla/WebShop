using System;
using System.Text.RegularExpressions;
using WebShop.Data;

namespace WebShop.Services.ProductParser
{
    public class ProductParser : IProductParser
    {
        private readonly Regex _deliveredInRegEx = new Regex("(\\d)(-(\\d))? (\\w*)", RegexOptions.Compiled);
        private const string Delimiter = ",";
        private const int ExpectedAmountOfItems = 10;

        public ProductParsingResult Parse(string line)
        {
            // format: Key,Artikelcode,colorcode,description,price,discountprice,delivered in,q1,size,color
            var parts = line.Split(Delimiter);
            if (parts.Length != ExpectedAmountOfItems)
            {
                return ProductParsingResult.Error(
                    $"Wrong amount of delimiter is the line. Please check how many '{Delimiter}' is in the line. Should be {ExpectedAmountOfItems - 1}"); 
            }

            try
            {
                var deliveredRange = ParseDeliveryRange(parts[6]);

                var product = new Product
                {
                    Key = ParseKey(parts[0]),
                    ArticleCode = parts[1],
                    ColorCode = parts[2],
                    Description = parts[3],
                    Price = ParsePrice(parts[4]),
                    DiscountPrice = ParseDiscountPrice(parts[5]),
                    DeliveredRangeFrom = deliveredRange.from,
                    DeliveredRangeTo = deliveredRange.to,
                    Q1 = parts[7],
                    Size = ParseSize(parts[8]),
                    Color = parts[9]
                };

                return ProductParsingResult.Ok(product);
            }
            catch (FormatException ex)
            {
                return ProductParsingResult.Error(ex.Message);
            }
        }

        private uint ParseSize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new FormatException("'Size' is empty. Each product should have a size field. Please check the line");
            }

            if (!uint.TryParse(input, out var size))
            {
                throw new FormatException("'Size' has wrong format. Size should be a number. Please check the line");
            }

            return size;
        }

        private double? ParseDiscountPrice(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (!double.TryParse(input, out var value))
            {
                throw new FormatException("'DiscountPrice' has wrong format. DiscountPrice should be a number. Please check the line");
            }

            return value;
        }

        private double ParsePrice(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new FormatException("'Price' is empty. Each product should have a price field. Please check the line");
            }

            if (!double.TryParse(input, out var value))
            {
                throw new FormatException("'Price' has wrong format. Price should be a number. Please check the line");
            }

            return value;
        }

        private (TimeSpan from, TimeSpan to) ParseDeliveryRange(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new FormatException("'Delivered in' is empty. Each product should have a delivered in field. Please check the line");
            }

            input = input.Trim().ToLower();

            // regex approach is not the most efficient, could be replaced with simple string manipulation logic if needed. 
            var result = _deliveredInRegEx.Match(input);
            if (!result.Success || result.Groups.Count != 5)
            {
                throw new FormatException("'Delivered in' has wrong format. Expected format <number>-<number?> <period>. For example '1-3 werkdagen', '3 maanden'");
            }

            // we can parse without TryParse since regex already checked for proper format
            int firstInterval = int.Parse(result.Groups[1].Value);
            int secondInteval = firstInterval;
            if (result.Groups[3].Success)
            {
                secondInteval = int.Parse(result.Groups[3].Value);
            }

            var period = result.Groups[4].Value.Trim().ToLower();

            switch (period)
            {
                case "werkdagen":
                case "werkdag":
                    return (from: new TimeSpan(firstInterval, 0, 0), new TimeSpan(secondInteval, 0, 0));
                case "maanden":
                case "maand":
                    return (from: new TimeSpan(firstInterval * 30, 0, 0), new TimeSpan(secondInteval * 30, 0, 0));
                default:
                    throw new FormatException($"Cannot parse 'Delivered in' period. '{period}' is unknown");
            }
        }

        private string ParseKey(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new FormatException("Key is empty. Each product should have a key. Please check the line");
            }

            return input;
        }
    }
}