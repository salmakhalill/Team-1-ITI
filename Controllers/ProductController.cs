using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Models;
using Team_1_ITI.Services;

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
            {
                query = query.Where(p => p.ProductName.Contains(searchString) || p.SKU.Contains(searchString));
            }

            
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "Low Stock")
                {
                    query = query.Where(p => p.StockQuantity <= p.LowStockThreshold);
                }
                else if (status == "In Stock")
                {
                    query = query.Where(p => p.StockQuantity > p.LowStockThreshold);
                }
            }

            
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentStatus = status;
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryID", "CategoryName", categoryId);
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(products);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null) return NotFound();

            return View(product);
        }

       
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,SKU,ProductName,CategoryID,UnitPrice,StockQuantity,LowStockThreshold,ReorderLevel")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,SKU,ProductName,CategoryID,UnitPrice,StockQuantity,LowStockThreshold,ReorderLevel")] Product product)
        {
            if (id != product.ProductID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null) return NotFound();

            return View(product);
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
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}