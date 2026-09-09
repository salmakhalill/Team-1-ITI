using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Team_1_ITI.Services;
using Team_1_ITI.ViewModels.Inventory;

namespace Team_1_ITI.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task<IActionResult> Index(
            string filter = "All",
            string search = "",
            int pageNumber = 1)
        {
            int pageSize = 10;

            var allProducts = await _inventoryService.GetAllProductsAsync();
            var products = await _inventoryService.GetProductsByStockStatusAsync(filter);

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products
                    .Where(p =>
                        p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.SKU.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int totalItems = products.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages && totalPages > 0) pageNumber = totalPages;

            var pagedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new InventoryIndexViewModel
            {
                Products = pagedProducts,

                TotalSKUs = allProducts.Count,

                InStockCount = allProducts.Count(p =>
                    p.StockQuantity > p.LowStockThreshold),

                LowStockCount = allProducts.Count(p =>
                    p.StockQuantity > 0 &&
                    p.StockQuantity <= p.LowStockThreshold),

                OutOfStockCount = allProducts.Count(p =>
                    p.StockQuantity == 0),

                CurrentFilter = filter,
                SearchTerm = search,

                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LowStock()
        {
            var products = await _inventoryService.GetLowStockProductsAsync();
            return View(products);
        }
    }
}