namespace Team_1_ITI.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        // KPI
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockCount { get; set; }

        public decimal TotalPurchases { get; set; }
        public decimal TotalSales { get; set; }
        public decimal StockValue { get; set; }

        public List<MonthlySalesViewModel> MonthlySales { get; set; } = new();

        public List<LowStockViewModel> LowStockProducts { get; set; } = new();

        public List<RecentActivityViewModel> RecentActivities { get; set; } = new();

        public List<MostSoldProductViewModel> MostSoldProducts { get; set; } = new();
    }


    public class MonthlySalesViewModel
    {
        public string Month { get; set; } = "";
        public decimal Amount { get; set; }
    }


    public class LowStockViewModel
    {
        public string ProductName { get; set; } = "";
        public string SKU { get; set; } = "";

        public int StockQuantity { get; set; }

        public int MinimumStock { get; set; }
    }


    public class RecentActivityViewModel
    {
        public string Type { get; set; } = "";

        public string Reference { get; set; } = "";

        public string Party { get; set; } = "";

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }
    }


    public class MostSoldProductViewModel
    {
        public string ProductName { get; set; } = "";

        public int QuantitySold { get; set; }
    }
}
