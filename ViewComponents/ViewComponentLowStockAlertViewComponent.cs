using Microsoft.AspNetCore.Mvc;
using Team_1_ITI.Services;

namespace Team_1_ITI.Components
{
    public class LowStockAlertViewComponent : ViewComponent
    {
        private readonly InventoryService _inventoryService;

        public LowStockAlertViewComponent(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Using GetProductsByStockStatusAsync instead of GetLowStockProductsAsync:
            // the latter's condition (StockQuantity <= LowStockThreshold) currently also
            // matches StockQuantity == 0, which would double-count out-of-stock items here.
            var lowStock = await _inventoryService.GetProductsByStockStatusAsync("LowStock");
            var outOfStock = await _inventoryService.GetProductsByStockStatusAsync("OutOfStock");

            var count = lowStock.Count + outOfStock.Count;

            return View(count);
        }
    }
}
