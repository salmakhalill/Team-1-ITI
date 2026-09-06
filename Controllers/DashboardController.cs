using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Data;
using Team_1_ITI.ModelView;

namespace Team_1_ITI.Controllers
{
    public class DashboardController : Controller
    {
        private readonly InventoryManagementDbContext context;

        public DashboardController(InventoryManagementDbContext context)
        {
            this.context = context;
        }


        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            model.TotalProducts = await context.Products.CountAsync();


            model.TotalCategories =await context.Categories.CountAsync();


            model.TotalSuppliers =await context.Suppliers.CountAsync();


            model.StockQuantity =await context.Products.SumAsync(p => p.StockQuantity);


            model.LowStockCount = await context.Products .CountAsync(p => p.StockQuantity <= p.LowStockThreshold);


            model.TotalPurchases = await context.Purchases.SumAsync(p => p.TotalAmount);


            model.TotalSales = await context.Sales .SumAsync(s => s.TotalAmount);


            model.StockValue = await context.Products.SumAsync(p =>p.StockQuantity * p.UnitPrice);

            model.LowStockProducts =
                await context.Products.Where(p => p.StockQuantity <= p.LowStockThreshold)
                    .OrderBy(p => p.StockQuantity)
                    .Take(5)
                    .Select(p => new LowStockViewModel
                    {
                        ProductName = p.ProductName,
                        SKU = p.SKU,
                        StockQuantity = p.StockQuantity,
                        MinimumStock = p.LowStockThreshold
                    })
                    .ToListAsync();

            var today = DateTime.Now;

            for (int i = 5; i >= 0; i--)
            {
                var date = today.AddMonths(-i);

                int month = date.Month;
                int year = date.Year;

                decimal amount = await context.Sales.
                       Where(s =>
                            s.SaleDate.Month == month &&
                            s.SaleDate.Year == year)
                       .SumAsync(s =>(decimal?)s.TotalAmount) ?? 0;

                model.MonthlySales.Add(new MonthlySalesViewModel
                    {
                        Month = date.ToString("MMM"),
                        Amount = amount
                    });
            }


            model.MostSoldProducts = await context.SaleItems
                    .Include(x => x.Product)
                    .GroupBy(x => new
                    {
                        x.ProductID,
                        x.Product.ProductName
                    })
                    .Select(g => new MostSoldProductViewModel
                    {
                        ProductName = g.Key.ProductName,
                        QuantitySold = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(x => x.QuantitySold)
                    .Take(5)
                    .ToListAsync();


            var recentSales =
                await context.Sales
                    .OrderByDescending(x => x.SaleDate)
                    .Take(6)
                    .Select(x => new RecentActivityViewModel
                    {
                        Type = "Sale",
                        Reference = "SO-" + x.SaleID,
                        Party = x.CustomerInfo,
                        Date = x.SaleDate,
                        Amount = x.TotalAmount
                    }).ToListAsync();


            var recentPurchases =
                await context.Purchases
                    .OrderByDescending(x => x.PurchaseDate)
                    .Take(6)
                    .Select(x => new RecentActivityViewModel
                    {
                        Type = "Purchase",
                        Reference = "PO-" + x.PurchaseID,
                        Party = x.Supplier.SupplierName,
                        Date = x.PurchaseDate,
                        Amount = x.TotalAmount
                    })
                    .ToListAsync();

            model.RecentActivities =recentSales.Concat(recentPurchases)
                    .OrderByDescending(x => x.Date)
                    .Take(6)
                    .ToList();


            return View("Index",model);
        }
    }
}
