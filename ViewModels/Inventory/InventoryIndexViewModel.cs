using Team_1_ITI.Models;

namespace Team_1_ITI.ViewModels.Inventory
{
    public class InventoryIndexViewModel
    {
        public List<Product> Products { get; set; } = new();

        public int TotalSKUs { get; set; }

        public int InStockCount { get; set; }

        public int LowStockCount { get; set; }

        public int OutOfStockCount { get; set; }

        public string CurrentFilter { get; set; } = "All";
        public string SearchTerm { get; set; } = "";
    }
}