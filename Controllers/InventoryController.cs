using Microsoft.AspNetCore.Mvc;
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
    string filter = "All")
        {
            var allProducts =
                await _inventoryService.GetAllProductsAsync();

            var products =
                await _inventoryService.GetProductsByStockStatusAsync(
                    filter);

            var model = new InventoryIndexViewModel
            {
                Products = products,

                TotalSKUs = allProducts.Count,

                InStockCount = allProducts.Count(p =>
                    p.StockQuantity > p.LowStockThreshold),

                LowStockCount = allProducts.Count(p =>
                    p.StockQuantity > 0 &&
                    p.StockQuantity <= p.LowStockThreshold),

                OutOfStockCount = allProducts.Count(p =>
                    p.StockQuantity == 0),

                CurrentFilter = filter
            };

            return View(model);
        }
    }
}