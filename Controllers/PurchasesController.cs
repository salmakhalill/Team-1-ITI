using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Data;
using Team_1_ITI.Services;
using Team_1_ITI.ViewModels.Purchases;

namespace Team_1_ITI.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly InventoryManagementDbContext _context;
        private readonly PurchaseService _purchaseService;

        public PurchasesController(InventoryManagementDbContext context, PurchaseService purchaseService)
        {
            _context = context;
            _purchaseService = purchaseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var model = await _purchaseService.GetPaginatedPurchasesAsync(page, pageSize);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _purchaseService.GetPurchaseDetailsAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? productId)
        {
            await PopulateDropDownsAsync();

            var model = new CreatePurchaseViewModel
            {
                Items = new List<PurchaseItemViewModel>
        {
            new PurchaseItemViewModel
            {
                ProductId = productId ?? 0
            }
        }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseViewModel model)
        {
            // Server-Side Validation
            if (model.Items != null && model.Items.Any())
            {
                bool supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierID == model.SupplierId);
                if (!supplierExists)
                {
                    ModelState.AddModelError("SupplierId", "The selected supplier does not exist or was deleted.");
                }

                var productIds = model.Items.Select(i => i.ProductId).ToList();
                var existingProductsCount = await _context.Products.CountAsync(p => productIds.Contains(p.ProductID));

                if (existingProductsCount != productIds.Count)
                {
                    ModelState.AddModelError("", "One or more selected products do not exist.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync();
                return View(model);
            }

            await _purchaseService.CreatePurchaseAsync(model);
            TempData["SuccessMessage"] = "Purchase recorded successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropDownsAsync()
        {
            ViewBag.Suppliers = await _context.Suppliers.ToListAsync();
            ViewBag.Products = await _context.Products.ToListAsync();
        }
    }
}