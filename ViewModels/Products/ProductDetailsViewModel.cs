namespace Team_1_ITI.ViewModels.Products
{
    public class ProductDetailsViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }

        public string StockStatus => StockQuantity == 0 ? "Out of Stock" : (StockQuantity <= LowStockThreshold ? "Low Stock" : "In Stock");
    }
}