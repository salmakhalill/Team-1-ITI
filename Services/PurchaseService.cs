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
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == item.ProductId);

                if (product == null)
                    throw new Exception("Product not found.");

                var lineTotal = item.Quantity * item.UnitCost;

                total += lineTotal;

                var purchaseItem = new PurchaseItem
                {
                    ProductID = item.ProductId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                };

                purchase.PurchaseItems.Add(purchaseItem);

                // Purchase Adjustment
                product.StockQuantity += item.Quantity;
            }

            purchase.TotalAmount = total;

            _context.Purchases.Add(purchase);

            await _context.SaveChangesAsync();
        }
    }
}