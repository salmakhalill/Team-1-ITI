namespace Team_1_ITI.ViewModels.Purchases
{
    public class PurchaseDetailsViewModel
    {
        public int PurchaseID { get; set; }
        public string SupplierName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseItemDetailsViewModel> Items { get; set; } = new();
    }

    public class PurchaseItemDetailsViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
    }
}