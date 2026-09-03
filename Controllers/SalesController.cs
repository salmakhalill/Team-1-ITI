using Microsoft.AspNetCore.Mvc;
using Team_1_ITI.Models;
using Team_1_ITI.Services;
using Team_1_ITI.ViewModels.Sales;

namespace Team_1_ITI.Controllers
{
    public class SalesController : Controller
    {
        private readonly SaleService service;

        public SalesController(SaleService saleService)
        {
            service = saleService;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            int pageSize = 10;

            var sales = service.GetPaged(
                pageNumber,
                pageSize,
                out int totalPages
            );

            SaleIndexViewModel model = new()
            {
                Sales = sales,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = service.GetProducts();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            SaleCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = service.GetProducts();

                return View(model);
            }

            var items = model.Items
                .Select(item => new SaleItem
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity
                })
                .ToList();

            bool success = service.CreateSale(
                model.CustomerInfo,
                items,
                out string errorMessage
            );

            if (!success)
            {
                ModelState.AddModelError(
                    "",
                    errorMessage
                );

                ViewBag.Products =
                    service.GetProducts();

                return View(model);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Details(int? SaleID)
        {
            if (SaleID == null)
                return NotFound();

            var sale = service.GetById(
                SaleID.Value
            );

            if (sale == null)
                return NotFound();

            return View(sale);
        }
    }
}