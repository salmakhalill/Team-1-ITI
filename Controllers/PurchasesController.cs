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

        public PurchasesController(
            InventoryManagementDbContext context,
            PurchaseService purchaseService)
        {
            _context = context;
            _purchaseService = purchaseService;
        }

        // GET: /Purchases/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Suppliers = await _context.Suppliers
                .ToListAsync();

            ViewBag.Products = await _context.Products
                .ToListAsync();

            return View(new CreatePurchaseViewModel());
        }

        // POST: /Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreatePurchaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = await _context.Suppliers
                    .ToListAsync();

                ViewBag.Products = await _context.Products
                    .ToListAsync();

                return View(model);
            }

            await _purchaseService.CreatePurchaseAsync(model);

            return RedirectToAction(nameof(Index));
        }

        // GET: /Purchases
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .OrderBy(p => p.PurchaseID)
                .ToListAsync();

            return View(purchases);
        }

        // GET: /Purchases/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.PurchaseID == id);

            if (purchase == null)
                return NotFound();

            return View(purchase);
        }
    }
}