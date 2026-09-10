using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Models;
using Team_1_ITI.Data;
using Team_1_ITI.ViewModels.Products;

namespace Team_1_ITI.Controllers
{
    public class ProductController : Controller
    {
        private readonly InventoryManagementDbContext _context;

        public ProductController(InventoryManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? categoryId, string status, int pageNumber = 1)
        {
            int pageSize = 10;
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
                query = query.Where(p => p.ProductName.Contains(searchString) || p.SKU.Contains(searchString));

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(p => p.CategoryID == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "Out of Stock")
                    query = query.Where(p => p.StockQuantity == 0);
                else if (status == "Low Stock")
                    query = query.Where(p => p.StockQuantity > 0 && p.StockQuantity <= p.LowStockThreshold);
                else if (status == "In Stock")
                    query = query.Where(p => p.StockQuantity > p.LowStockThreshold);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var dbProducts = await query
                .OrderBy(p => p.ProductID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ProductIndexViewModel
            {
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                CurrentSearch = searchString,
                CurrentCategory = categoryId,
                CurrentStatus = status,
                Products = dbProducts.Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    SKU = p.SKU,
                    CategoryID = p.CategoryID,
                    UnitPrice = p.UnitPrice,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold
                }).ToList()
            };

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryID", "CategoryName", categoryId);
            return View(viewModel);
        }

        public IActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            bool isSkuExists = await _context.Products.AnyAsync(p => p.SKU == model.SKU);
            if (isSkuExists)
            {
                ModelState.AddModelError("SKU", "This SKU is already registered.");
            }

            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductName = model.ProductName,
                    SKU = model.SKU,
                    CategoryID = model.CategoryID,
                    UnitPrice = model.UnitPrice,
                    StockQuantity = model.StockQuantity,
                    LowStockThreshold = model.LowStockThreshold
                };

                _context.Add(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product added successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryID = new SelectList(_context.Categories, "CategoryID", "CategoryName", model.CategoryID);
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var model = new ProductViewModel
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                SKU = product.SKU,
                CategoryID = product.CategoryID,
                UnitPrice = product.UnitPrice,
                StockQuantity = product.StockQuantity,
                LowStockThreshold = product.LowStockThreshold
            };

            ViewBag.CategoryID = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            bool isSkuExists = await _context.Products.AnyAsync(p => p.SKU == model.SKU && p.ProductID != model.ProductID);
            if (isSkuExists)
            {
                ModelState.AddModelError("SKU", "This SKU is already registered for another product.");
            }

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(model.ProductID);
                if (product == null) return NotFound();

                product.ProductName = model.ProductName;
                product.SKU = model.SKU;
                product.CategoryID = model.CategoryID;
                product.UnitPrice = model.UnitPrice;
                product.StockQuantity = model.StockQuantity;
                product.LowStockThreshold = model.LowStockThreshold;

                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryID = new SelectList(_context.Categories, "CategoryID", "CategoryName", model.CategoryID);
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                SKU = product.SKU,
                CategoryName = product.Category?.CategoryName ?? "N/A",
                UnitPrice = product.UnitPrice,
                StockQuantity = product.StockQuantity,
                LowStockThreshold = product.LowStockThreshold
            };

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                SKU = product.SKU,
                CategoryName = product.Category?.CategoryName ?? "N/A",
                UnitPrice = product.UnitPrice,
                StockQuantity = product.StockQuantity,
                LowStockThreshold = product.LowStockThreshold
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        //Client-Side Validation [Remote]
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckSKU(string SKU, int ProductID = 0)
        {
            bool isExists = await _context.Products
                .AnyAsync(p => p.SKU == SKU && p.ProductID != ProductID);

            if (isExists)
            {
                return Json(false);
            }
            return Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckProductName(string ProductName, int ProductID = 0)
        {
            bool isExists = await _context.Products
                .AnyAsync(p => p.ProductName == ProductName && p.ProductID != ProductID);

            if (isExists)
            {
                return Json(false);
            }
            return Json(true);
        }
    }
}