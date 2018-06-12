using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebShop.Models
{    
    // Class to simplify work with paginated results (since we could have quite a lot of imported items)
    public class PaginatedList<T> : List<T>
    {
        private const int PageSize = 20;

        public int TotalAmount { get; }
        public int PageNumber { get;  }
        public int TotalPages { get; }

        public PaginatedList(List<T> items, int count, int pageNumber)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (float)PageSize);
            TotalAmount = count;
            AddRange(items);
        }

        public bool HasPreviousPage => PageNumber > 1;
        
        public bool HasNextPage => PageNumber < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber)
        {
            var count = await source.CountAsync();
            var items = await source
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            return new PaginatedList<T>(items, count, pageNumber);
        }
    }
}