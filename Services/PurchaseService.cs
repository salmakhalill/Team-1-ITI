using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Data;
using Team_1_ITI.Models;
using Team_1_ITI.ViewModels.Purchases;

namespace Team_1_ITI.Services
{
    public class PurchaseService
    {
        private readonly InventoryManagementDbContext _context;

        public PurchaseService(InventoryManagementDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseIndexViewModel> GetPaginatedPurchasesAsync(int page, int pageSize)
        {
            var totalCount = await _context.Purchases.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .OrderBy(p => p.PurchaseID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PurchaseListViewModel
                {
                    PurchaseID = p.PurchaseID,
                    SupplierName = p.Supplier.SupplierName,
                    PurchaseDate = p.PurchaseDate,
                    TotalAmount = p.TotalAmount
                })
                .ToListAsync();

            return new PurchaseIndexViewModel
            {
                Purchases = purchases,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task<PurchaseDetailsViewModel?> GetPurchaseDetailsAsync(int id)
        {
            return await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Product)
                .Where(p => p.PurchaseID == id)
                .Select(p => new PurchaseDetailsViewModel
                {
                    PurchaseID = p.PurchaseID,
                    SupplierName = p.Supplier.SupplierName,
                    PurchaseDate = p.PurchaseDate,
                    TotalAmount = p.TotalAmount,
                    Items = p.PurchaseItems.Select(i => new PurchaseItemDetailsViewModel
                    {
                        ProductName = i.Product.ProductName,
                        Quantity = i.Quantity,
                        UnitCost = i.UnitCost,
                        LineTotal = i.Quantity * i.UnitCost
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreatePurchaseAsync(CreatePurchaseViewModel model)
        {
            var purchase = new Purchase
            {
                SupplierID = model.SupplierId,
                PurchaseDate = model.PurchaseDate,
                PurchaseItems = new List<PurchaseItem>()
            };

            decimal total = 0;

            foreach (var item in model.Items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == item.ProductId);
                if (product == null) throw new Exception("Product not found.");

                total += item.Quantity * item.UnitCost;

                purchase.PurchaseItems.Add(new PurchaseItem
                {
                    ProductID = item.ProductId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                });

                product.StockQuantity += item.Quantity;
            }

            purchase.TotalAmount = total;
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
        }
    }
}