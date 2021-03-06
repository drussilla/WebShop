﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WebShopContext _context;

        public ProductsController(WebShopContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? page)
        {
            if (page < 1)
            {
                page = 1;
            }

            // we could use separate DTO or ViewModel object with auto mapping to 
            // not expose Product entity to the view layer
            // For this example I just use Product to save some time
            return View(await PaginatedList<Product>.CreateAsync(_context.Products.AsNoTracking(), page ?? 1));
        }
    }
}
