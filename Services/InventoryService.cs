using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Data;
using Team_1_ITI.Models;

namespace Team_1_ITI.Services
{
    public class InventoryService
    {
        private readonly InventoryManagementDbContext _context;

        public InventoryService(InventoryManagementDbContext context)
        {
            _context = context;
        }

        // Get stock of a specific product
        public async Task<Product?> GetProductStockAsync(string productName)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductName.ToLower() == productName.ToLower());
        }

        // Get low-stock products
        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Where(p => p.StockQuantity <= p.LowStockThreshold)
                .ToListAsync();
        }

        // Get out-of-stock products
        public async Task<List<Product>> GetOutOfStockProductsAsync()
        {
            return await _context.Products
                .Where(p => p.StockQuantity == 0)
                .ToListAsync();
        }

        // Get best-selling products
        public async Task<List<object>> GetBestSellingProductsAsync()
        {
            return await _context.SaleItems
                .GroupBy(si => new
                {
                    si.ProductID,
                    si.Product.ProductName
                })
                .Select(g => new
                {
                    productName = g.Key.ProductName,
                    totalQuantitySold = g.Sum(si => si.Quantity)
                })
                .OrderByDescending(x => x.totalQuantitySold)
                .Take(5)
                .Cast<object>()
                .ToListAsync();
        }

        // Get recent purchases
        public async Task<List<object>> GetRecentPurchasesAsync()
        {
            return await _context.Purchases
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.PurchaseDate)
                .Take(5)
                .Select(p => new
                {
                    purchaseID = p.PurchaseID,
                    supplierName = p.Supplier.SupplierName,
                    purchaseDate = p.PurchaseDate,
                    totalAmount = p.TotalAmount
                })
                .Cast<object>()
                .ToListAsync();
        }

        // Get recent sales
        public async Task<List<object>> GetRecentSalesAsync()
        {
            return await _context.Sales
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .Select(s => new
                {
                    saleID = s.SaleID,
                    saleDate = s.SaleDate,
                    totalAmount = s.TotalAmount,
                    customerInfo = s.CustomerInfo
                })
                .Cast<object>()
                .ToListAsync();
        }

        // Get all suppliers
        public async Task<List<object>> GetSuppliersAsync()
        {
            return await _context.Suppliers
                .OrderBy(s => s.SupplierName)
                .Select(s => new
                {
                    supplierID = s.SupplierID,
                    supplierName = s.SupplierName,
                    contactName = s.ContactName
                })
                .Cast<object>()
                .ToListAsync();
        }
    }
}