using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Data;
using Team_1_ITI.Models;

namespace Team_1_ITI.Services
{
    public class SaleService
    {
        private readonly InventoryManagementDbContext db;

        public SaleService(InventoryManagementDbContext db)
        {
            this.db = db;
        }

        public List<Sale> GetPaged(
            int pageNumber,
            int pageSize,
            out int totalPages)
        {
            int totalSales = db.Sales.Count();

            totalPages = (int)Math.Ceiling(
                (double)totalSales / pageSize
            );

            if (pageNumber < 1)
                pageNumber = 1;

            if (pageNumber > totalPages && totalPages > 0)
                pageNumber = totalPages;

            return db.Sales
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .OrderByDescending(s => s.SaleDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Sale? GetById(int id)
        {
            return db.Sales
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefault(s => s.SaleID == id);
        }

        public List<Product> GetProducts()
        {
            return db.Products
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        public bool CreateSale(
            string? customerInfo,
            List<SaleItem> items,
            out string errorMessage)
        {
            errorMessage = "";

            if (items == null || items.Count == 0)
            {
                errorMessage =
                    "A sale must contain at least one product.";

                return false;
            }

            decimal totalAmount = 0;

            foreach (var item in items)
            {
                var product = db.Products
                    .FirstOrDefault(
                        p => p.ProductID == item.ProductID
                    );

                if (product == null)
                {
                    errorMessage = "Product not found.";
                    return false;
                }

                if (item.Quantity <= 0)
                {
                    errorMessage =
                        "Quantity must be greater than zero.";

                    return false;
                }

                if (product.StockQuantity < item.Quantity)
                {
                    errorMessage =
                        $"Not enough stock for product: {product.ProductName}";

                    return false;
                }

                item.UnitPrice = product.UnitPrice;

                totalAmount +=
                    item.Quantity * item.UnitPrice;
            }

            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                CustomerInfo = customerInfo,
                TotalAmount = totalAmount,
                SaleItems = new List<SaleItem>()
            };

            foreach (var item in items)
            {
                var product = db.Products
                    .First(
                        p => p.ProductID == item.ProductID
                    );

                product.StockQuantity -= item.Quantity;

                sale.SaleItems.Add(new SaleItem
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            db.Sales.Add(sale);

            db.SaveChanges();

            return true;
        }
    }
}